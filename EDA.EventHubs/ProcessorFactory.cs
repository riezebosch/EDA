using Azure.Messaging.EventHubs;
using EDA.Ports;

namespace EDA.EventHubs
{
    public static class ProcessorFactory
    {
        public static IProcessor AttachTo<T>(this EventProcessorClient processor, ISubscribe<T> service, string @event) => 
            new Processor<T>(processor, service, @event);
        
        public static ITestProcessor<T> AttachTo<T>(this EventProcessorClient processor, string @event) => 
            new TestProcessor<T>(processor, @event);
    }
}