using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using EDA.Ports;

namespace EDA.ServiceBus
{
    public static class ServiceBusProcessorExt
    {
        public static ServiceBusProcessor Hookup<T>(this ServiceBusProcessor processor, ISubscribe<T> service)
        {
            processor.ProcessMessageAsync +=
                e => service.Handle(e.Message.FromEvent<T>());

            processor.ProcessErrorAsync += 
                _ => Task.CompletedTask;

            return processor;
        }
    }
}