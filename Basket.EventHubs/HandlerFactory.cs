using Azure.Messaging.EventHubs;

namespace Basket.EventHubs
{
    public static class HandlerFactory
    {
        public static IHookup Hookup<T>(this EventProcessorClient processor, IHandle<T> service, string @event) => 
            new Hookup<T>(processor, service, @event);
        
        public static ITestHookup<T> Hookup<T>(this EventProcessorClient processor, string @event) => 
            new TestHookup<T>(processor, @event);
    }
}