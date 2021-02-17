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
            
            handler.Assert(x => x.Should().Be("input"), TimeSpan.Zero);
        }
        
        [Fact]
        public void Timeout()
        {
            var handler = new Subscriber<string>();
            handler.Invoking(x => x.Assert(y => y.Should().Be("input"), TimeSpan.Zero))
                .Should()
                .Throw<TimeoutException>();
        }
        
        [Fact]
        public async Task Fails()
        {
            var handler = new Subscriber<string>();
            await handler.As<ISubscribe<string>>().Handle("other");
            
            handler.Invoking(x => x.Assert(y => y.Should().Be("input"), TimeSpan.Zero))
                .Should()
                .Throw<XunitException>();
        }
        
        [Fact]
        public async Task FailsTwice()
        {
            var handler = new Subscriber<string>();
            await handler.As<ISubscribe<string>>().Handle("other");
            await handler.As<ISubscribe<string>>().Handle("different");
            
            handler.Invoking(x => x.Assert(y => y.Should().Be("input"), TimeSpan.Zero))
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

            handler.Assert(y => y.Should().Be("then"), TimeSpan.Zero);
        }
    }
}