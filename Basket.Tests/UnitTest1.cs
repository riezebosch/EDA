using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Bogus;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Basket.Tests
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
                var order = NewOrder();
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

            var done = new TaskCompletionSource();
            await using var reader = _bus.CreateProcessor("Orders", "a", new ServiceBusProcessorOptions());
            reader.ProcessMessageAsync += e => 
            {
                if (e.Message.Body.ToString() == input)
                    done.SetResult();

                return Task.CompletedTask;
            };

            reader.ProcessErrorAsync += e => Task.CompletedTask;

            await reader.StartProcessingAsync();
            using var tcs = new CancellationTokenSource(TimeSpan.FromMinutes(5));
            var token = tcs.Token;
            token.Register(() => done.SetCanceled(token));

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

        private static Order NewOrder()
        {
            var item = new Faker<Item>()
                .StrictMode(true)
                .RuleFor(i => i.Name, f => f.Commerce.Product())
                .RuleFor(i => i.Quantity, f => f.Random.Number(10))
                .RuleFor(i => i.Price, f => 0.01m * f.Random.Number(10, 3000));

            var order = new Faker<Order>()
                .StrictMode(true)
                .RuleFor(x => x.Id, f => f.UniqueIndex)
                .RuleFor(x => x.Email, f => f.Internet.Email())
                .RuleFor(x => x.Items, f => item.Generate(5));

            return order.Generate();
        }

        public async Task InitializeAsync()
        {
        }

        public async Task DisposeAsync() => await _bus.DisposeAsync();
    }

    public class Order
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public IEnumerable<Item> Items { get; set; }
    }

    public class Item
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}