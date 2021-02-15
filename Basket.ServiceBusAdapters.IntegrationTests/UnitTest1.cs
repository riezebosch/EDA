using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Basket.ServiceBusAdapters.IntegrationTests
{
    public class UnitTest1 : IAsyncLifetime
    {
        private readonly ServiceBusClient _bus;

        public UnitTest1()
        {
            var config = new ConfigurationBuilder()
                .AddUserSecrets<UnitTest1>()
                .Build();

            _bus = new ServiceBusClient(config["Azure:ServiceBus:ConnectionString"]);
        }
        [Fact]
        public async Task Test1()
        {
            var sender = _bus.CreateSender("Orders");
            
            for (var i = 0; i < 30; i++)
            {
                var order = Generator.NewOrder();
                var messages = new []
                {
                    CreateMessage(order, "OrderCreated"), 
                    CreateMessage(new { order.Id }, "OrderShipped"),
                    CreateMessage(new { order.Id, TotalAmount = order.Items.Select(x => x.Price * x.Quantity).Sum(), Date = DateTime.Now }, "InvoiceSent"),
                    CreateMessage(new { order.Id }, "OrderDelivered"),
                    CreateMessage(new { order.Id }, "InvoicePayed")
                };

                await SetScheduleFor(sender, messages);
            }
        }

        [Fact]
        public async Task Scheduled()
        {
            var schedule = DateTime.UtcNow.AddMinutes(2);

            var input = Guid.NewGuid().ToString();
            await using var sender = _bus.CreateSender("Orders");
            await sender.SendMessageAsync(new ServiceBusMessage(input)
            {
                ScheduledEnqueueTime = schedule
            });

            var done = new TaskCompletionSource<ServiceBusReceivedMessage>();
            await using var reader = _bus.CreateProcessor("Orders", "a", new ServiceBusProcessorOptions());
            reader.ProcessMessageAsync += e => 
            {
                if (e.Message.Body.ToString() == input)
                    done.SetResult(e.Message);

                return Task.CompletedTask;
            };

            reader.ProcessErrorAsync += _ => Task.CompletedTask;

            await reader.StartProcessingAsync();
            using var tcs = new CancellationTokenSource(TimeSpan.FromMinutes(5));
            var token = tcs.Token;
            token.Register(() => done.SetCanceled());

            await done.Task;
            DateTime.UtcNow.Should().BeAfter(schedule);
        }

        private static async Task SetScheduleFor(ServiceBusSender sender, ServiceBusMessage[] messages)
        {
            var schedule = DateTimeOffset.UtcNow;
            var random = new Random(23523);
            
            foreach (var message in messages)
            {
                schedule = schedule.AddSeconds(random.Next(5, 30));
                await sender.ScheduleMessageAsync(message, schedule);
            }
        }

        private static ServiceBusMessage CreateMessage(object order, string type) =>
            new(JsonSerializer.Serialize(order))
            {
                Subject = type
            };

        public Task InitializeAsync() => Task.CompletedTask;

        public async Task DisposeAsync() => await _bus.DisposeAsync();
    }
}