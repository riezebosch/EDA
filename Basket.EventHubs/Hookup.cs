using System;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Processor;

namespace Basket.EventHubs
{
    internal class Hookup<T> : IHookup
    {
        private readonly EventProcessorClient _processor;
        private readonly IHandle<T> _handler;
        private readonly string _event;

        public Hookup(EventProcessorClient processor, IHandle<T> handler, string @event)
        {
            _processor = processor;
            _handler = handler;
            _event = @event;
        }

        public async Task Start()
        {
            _processor.ProcessEventAsync += Handle;
            _processor.ProcessErrorAsync += Handle;

            await _processor.StartProcessingAsync();
        }

        async ValueTask IAsyncDisposable.DisposeAsync() =>
            await Stop();

        private async Task Handle(ProcessEventArgs e)
        {
            if ((string) e.Data.Properties["event"] == _event)
            {
                await _handler.Handle(JsonSerializer.Deserialize<T>(e.Data.EventBody));
            }

            await e.UpdateCheckpointAsync();
        }
        
        private async Task Stop()
        {
            try
            {
                await _processor.StopProcessingAsync();
            }
            finally
            {
                _processor.ProcessEventAsync -= Handle;
                _processor.ProcessErrorAsync -= Handle;
            }
        }

        private static Task Handle(ProcessErrorEventArgs e) =>
            throw e.Exception;
    }
}