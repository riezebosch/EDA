using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Basket.EventHubs.IntegrationTests
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test1()
        {
            var config = new ConfigurationBuilder()
                .AddUserSecrets<UnitTest1>()
                .Build();

            var client = new EventHubProducerClient(config["Azure:EventHubs:ConnectionString"], "orders");
            await client.SendAsync(new []
            {
                new EventData(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(Generator.NewOrder())))
            });
        }
    }
}