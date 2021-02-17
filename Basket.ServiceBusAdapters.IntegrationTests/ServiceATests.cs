using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Basket.Events;
using Basket.Services;
using Basket.TestAdapters;
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
                .AddUserSecrets<ServiceATests>()
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
            await _bus
                .CreateProcessor("orders", "ServiceA")
                .Hookup(new ServiceA(publisher))
                .StartProcessingAsync();
            
            await _admin.Setup("orders", "test", "OrderCreated");

            using var test = new TestHandler<OrderCreated>();
            await _bus
                .CreateProcessor("orders", "test")
                .Hookup(test)
                .StartProcessingAsync();

            var order = Generator.NewOrder();
            await publisher.Publish( "CreateOrder", order, DateTimeOffset.UtcNow);

            test.Assert(x => x.Should().BeEquivalentTo(order), TimeSpan.FromSeconds(10));
        }

        public Task InitializeAsync() => 
            Task.CompletedTask;

        public async Task DisposeAsync() => 
            await _bus.DisposeAsync();
    }
}