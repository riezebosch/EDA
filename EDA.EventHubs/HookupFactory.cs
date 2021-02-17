using Azure.Messaging.EventHubs;
using EDA.Ports;

namespace EDA.EventHubs
{
    public static class HookupFactory
    {
        public static IHookup Hookup<T>(this EventProcessorClient processor, ISubscribe<T> service, string @event) => 
            new Hookup<T>(processor, service, @event);
        
        public static ITestHookup<T> Hookup<T>(this EventProcessorClient processor, string @event) => 
            new TestHookup<T>(processor, @event);
    }
}