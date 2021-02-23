using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using EDA.Ports;

namespace EDA.TestAdapters
{
    public sealed class Subscriber<T> : 
        ISubscribe<T>, 
        IAssert<T>
    {
        private readonly Channel<T> _messages = Channel.CreateUnbounded<T>();

        public async Task Assert(Action<T> assert, TimeSpan timeout)
        {
            var exceptions = new List<Exception>();

            try
            {
                using var source = new CancellationTokenSource();
                source.CancelAfter(timeout);

                await foreach (var message in _messages.Reader.ReadAllAsync(source.Token))
                {
                    try
                    {
                        assert(message);
                        return;
                    }
                    catch (Exception e)
                    {
                        exceptions.Add(e);
                    }

                    source.CancelAfter(timeout);
                }
            }
            catch (OperationCanceledException)
            {
                throw exceptions.Any() 
                    ? new AggregateException(exceptions) 
                    : new TimeoutException();
            }
        }

        public Task Assert(Action<T> assert) => 
            Assert(assert, TimeSpan.FromSeconds(60));

        public async Task Handle(T body) => 
            await _messages.Writer.WriteAsync(body);
    }
}