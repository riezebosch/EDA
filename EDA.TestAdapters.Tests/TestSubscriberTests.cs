using System;
using System.Threading.Tasks;
using EDA.Ports;
using FluentAssertions;
using Xunit;
using Xunit.Sdk;

namespace EDA.TestAdapters.Tests
{
    public class TestSubscriberTests
    {
        [Fact]
        public async Task Assert()
        {
            var handler = new Subscriber<string>();
            await handler.As<ISubscribe<string>>().Handle("input");
            
            await handler.Assert(x => x.Should().Be("input"), TimeSpan.FromSeconds(1));
        }
        
        [Fact]
        public void Timeout()
        {
            var handler = new Subscriber<string>();
            handler.Invoking(x => x.Assert(y => y.Should().Be("input"), TimeSpan.FromSeconds(1)))
                .Should()
                .Throw<TimeoutException>();
        }
        
        [Fact]
        public async Task ResetTimeoutOnMessageArrival()
        {
            var handler = new Subscriber<string>();
            async Task Emit()
            {
                var subscriber = handler.As<ISubscribe<string>>();
                for (var i = 0; i < 5; i++)
                {
                    await subscriber.Handle($"a{i}");
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }

                await subscriber.Handle("input");
            }

            await Task.WhenAll(Emit(), handler.Assert(y => y.Should().Be("input"), TimeSpan.FromSeconds(2)));
        }
        
        [Fact]
        public async Task Fails()
        {
            var handler = new Subscriber<string>();
            await handler.As<ISubscribe<string>>().Handle("other");
            
            handler.Invoking(x => x.Assert(y => y.Should().Be("input"), TimeSpan.FromSeconds(1)))
                .Should()
                .Throw<XunitException>();
        }
        
        [Fact]
        public async Task FailsTwice()
        {
            var handler = new Subscriber<string>();
            await handler.As<ISubscribe<string>>().Handle("other");
            await handler.As<ISubscribe<string>>().Handle("different");
            
            handler.Invoking(x => x.Assert(y => y.Should().Be("input"), TimeSpan.FromSeconds(1)))
                .Should()
                .Throw<AggregateException>()
                .Which
                .InnerExceptions
                .Count
                .Should()
                .Be(2);
        }
        
        [Fact]
        public async Task Retry()
        {
            var handler = new Subscriber<string>();
            await handler.As<ISubscribe<string>>().Handle("first");
            await handler.As<ISubscribe<string>>().Handle("then");

            await handler.Assert(y => y.Should().Be("then"), TimeSpan.FromSeconds(1));
        }
    }
}