using System;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Producer;
using Azure.Storage.Blobs;
using EDA.Example;
using EDA.Example.Events;
using EDA.Example.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace EDA.EventHubs.IntegrationTests
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

            var blob = new BlobServiceClient(_config["Azure:EventHubs:StorageConnectionString"]);
            await using var service = new EventProcessorClient(
                    await blob.SetupStore("service-a"),
                    EventHubConsumerClient.DefaultConsumerGroupName,
                    _config["Azure:EventHubs:ConnectionString"],
                    "orders")
                .Hookup(new ServiceA(publisher), "CreateOrder");
            await service.Start();

            await using var test = new EventProcessorClient(
                    await blob.SetupStore("test"),
                    EventHubConsumerClient.DefaultConsumerGroupName,
                    _config["Azure:EventHubs:ConnectionString"],
                    "orders")
                .Hookup<OrderCreated>("OrderCreated");
            await test.Start();

            test.Assert(x => x.Should().BeEquivalentTo(order));
        }
    }
}