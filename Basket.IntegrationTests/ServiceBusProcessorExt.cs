using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Basket.Commands;
using Basket.Services;

namespace Basket.IntegrationTests
{
    internal static class ServiceBusProcessorExt
    {
        public static ServiceBusProcessor Hookup(this ServiceBusProcessor processor, ServiceA service)
        {
            processor.ProcessMessageAsync +=
                e => service.Handle(JsonSerializer.Deserialize<CreateOrder>(e.Message.Body));

            processor.ProcessErrorAsync += e => Task.CompletedTask;

            return processor;
        }
    }
}