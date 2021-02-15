using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Basket.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Basket.ServiceBusAdapters.IntegrationTests
{
    public class ServiceATests : IAsyncLifetime
    {
        private readonly ServiceBusClient _bus;
        private readonly ServiceBusAdministrationClient _admin;

        public ServiceATests()
        {
            var config = new ConfigurationBuilder()
                .AddUserSecrets<UnitTest1>()
                .Build();

            _bus = new ServiceBusClient(config["Azure:ServiceBus:ConnectionString"]);
            _admin = new ServiceBusAdministrationClient(config["Azure:ServiceBus:ConnectionString"]);
        }

        [Fact]
        public async Task Test()
        {
            await using var sender = _bus.CreateSender("orders");
            var publisher = new Publisher(sender); 
            
            await _admin.Setup("orders", "ServiceA", "CreateOrder");
            await using var processor = _bus.CreateProcessor("orders", "ServiceA").Hookup(new ServiceA(publisher));
            await processor.StartProcessingAsync();
            
            await _admin.Setup("orders", "test", "OrderCreated");
            await using var test = _bus.CreateProcessor("orders", "test");

            using var results = new BlockingCollection<ServiceBusReceivedMessage>();

            test.ProcessMessageAsync += async e => results.Add(e.Message);
            test.ProcessErrorAsync += _ => Task.CompletedTask;
            await test.StartProcessingAsync();
            
            await publisher.Publish( "CreateOrder", Generator.NewOrder(), DateTimeOffset.UtcNow);

            results.TryTake(out var message, TimeSpan.FromMinutes(1)).Should().BeTrue();
            message!.Subject.Should().Be("OrderCreated");
        }

        public Task InitializeAsync() => Task.CompletedTask;

        public async Task DisposeAsync() => await _bus.DisposeAsync();
    }
}