using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace Basket.ServiceBusAdapters
{
    public static class ServiceBusProcessorExt
    {
        public static ServiceBusProcessor Hookup<T>(this ServiceBusProcessor processor, IHandle<T> service)
        {
            processor.ProcessMessageAsync +=
                e => service.Handle(JsonSerializer.Deserialize<T>(e.Message.Body));

            processor.ProcessErrorAsync += _ => Task.CompletedTask;

            return processor;
        }
    }
}