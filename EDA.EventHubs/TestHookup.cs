using System;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using EDA.TestAdapters;

namespace EDA.EventHubs
{
    internal class TestHookup<T> : ITestHookup<T>
    {
        private readonly IHookup _hookup;
        private readonly Subscriber<T> _subscriber = new();

        public TestHookup(EventProcessorClient processor, string @event) =>
            _hookup = new Hookup<T>(processor, _subscriber, @event);

        public async Task Start() => 
            await _hookup.Start();

        public async ValueTask DisposeAsync()
        {
            await _hookup.DisposeAsync();
            _subscriber.Dispose();
        }

        public void Assert(Action<T> assert, TimeSpan timeout) => 
            _subscriber.Assert(assert, timeout);

        public void Assert(Action<T> assert) => 
            _subscriber.Assert(assert);
    }
}