using System;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Producer;
using Azure.Storage.Blobs;
using Basket.Events;
using Basket.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Basket.EventHubs.IntegrationTests
{
    public class UnitTest1
    {
        private readonly IConfigurationRoot _config;

        public UnitTest1() =>
            _config = new ConfigurationBuilder()
                .AddUserSecrets<UnitTest1>()
                .Build();

        [Fact]
        public async Task Test1()
        {
            var order = Generator.NewOrder();

            await using var client = new EventHubProducerClient(_config["Azure:EventHubs:ConnectionString"], "orders");
            var publisher = new Publisher(client);
            await publisher.Publish("CreateOrder", order, DateTimeOffset.UtcNow);
            
            var processor = new EventProcessorClient(
                new BlobContainerClient(_config["Azure:EventHubs:StorageConnectionString"], "servicea"),
                EventHubConsumerClient.DefaultConsumerGroupName, _config["Azure:EventHubs:ConnectionString"], "orders")
                .Hookup(new ServiceA(publisher), "CreateOrder");
            await processor.StartProcessingAsync();

            var consumer = new TestHandler<OrderCreated>();
            var test = new EventProcessorClient(
                new BlobContainerClient(_config["Azure:EventHubs:StorageConnectionString"], "test"), 
                EventHubConsumerClient.DefaultConsumerGroupName, _config["Azure:EventHubs:ConnectionString"], "orders")
                .Hookup(consumer, "OrderCreated");
            await test.StartProcessingAsync();

            try
            {
                consumer.Assert(x => x.Should().BeEquivalentTo(order));
            }
            finally
            {
                await processor.StopProcessingAsync();
                await test.StopProcessingAsync();
            }
        }
    }
}