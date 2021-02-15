using System;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Producer;
using Azure.Storage.Blobs;
using Basket.Events;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Basket.EventHubs.IntegrationTests
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test1()
        {
            var order = Generator.NewOrder();

            var config = new ConfigurationBuilder()
                .AddUserSecrets<UnitTest1>()
                .Build();

            var client = new EventHubProducerClient(config["Azure:EventHubs:ConnectionString"], "orders");
            await client.SendAsync(new []
            {
                new EventData(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(Generator.NewOrder())))
            });

            var messages = new BlockingCollection<EventData>();
            var processor = new EventProcessorClient(new BlobContainerClient(config["Azure:EventHubs:StorageConnectionString"], "test"), EventHubConsumerClient.DefaultConsumerGroupName, config["Azure:EventHubs:ConnectionString"], "orders");
            processor.ProcessEventAsync += async e =>
            {
                messages.Add(e.Data);
                await e.UpdateCheckpointAsync();
            };

            processor.ProcessErrorAsync += e => throw e.Exception;
            await processor.StartProcessingAsync();

            messages.TryTake(out var result, TimeSpan.FromSeconds(30)).Should().BeTrue();
            JsonSerializer.Deserialize<OrderCreated>(result!.EventBody).Should().BeEquivalentTo(order);
            
        }
    }
}