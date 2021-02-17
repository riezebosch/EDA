using System;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Basket.TestAdapters;

namespace Basket.EventHubs
{
    internal class TestHookup<T> : ITestHookup<T>
    {
        private readonly IHookup _hookup;
        private readonly TestHandler<T> _handler = new();

        public TestHookup(EventProcessorClient processor, string @event) =>
            _hookup = new Hookup<T>(processor, _handler, @event);

        public async Task Start() => 
            await _hookup.Start();

        public async ValueTask DisposeAsync()
        {
            await _hookup.DisposeAsync();
            _handler.Dispose();
        }

        public void Assert(Action<T> assert, TimeSpan timeout) => 
            _handler.Assert(assert, timeout);

        public void Assert(Action<T> assert) => 
            _handler.Assert(assert);
    }
}