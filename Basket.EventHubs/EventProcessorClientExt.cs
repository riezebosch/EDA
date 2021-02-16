using System.Text.Json;
using Azure.Messaging.EventHubs;

namespace Basket.EventHubs
{
    public static class EventProcessorClientExt
    {
        public static EventProcessorClient Hookup<T>(this EventProcessorClient processor, IHandle<T> service, string @event)
        {
            processor.ProcessEventAsync += async e =>
            {
                if ((string) e.Data.Properties["event"] == @event)
                {
                    await service.Handle(JsonSerializer.Deserialize<T>(e.Data.EventBody));
                }

                await e.UpdateCheckpointAsync();
            };
            
            
            processor.ProcessErrorAsync += e => throw e.Exception;
            return processor;
        }
    }
}