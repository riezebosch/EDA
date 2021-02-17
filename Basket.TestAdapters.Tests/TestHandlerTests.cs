using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Xunit.Sdk;

namespace Basket.TestAdapters.Tests
{
    public class TestHandlerTests
    {
        [Fact]
        public async Task Assert()
        {
            var handler = new TestHandler<string>();
            await handler.As<IHandle<string>>().Handle("input");
            
            handler.Assert(x => x.Should().Be("input"), TimeSpan.Zero);
        }
        
        [Fact]
        public void Timeout()
        {
            var handler = new TestHandler<string>();
            handler.Invoking(x => x.Assert(y => y.Should().Be("input"), TimeSpan.Zero))
                .Should()
                .Throw<TimeoutException>();
        }
        
        [Fact]
        public async Task Fails()
        {
            var handler = new TestHandler<string>();
            await handler.As<IHandle<string>>().Handle("other");
            
            handler.Invoking(x => x.Assert(y => y.Should().Be("input"), TimeSpan.Zero))
                .Should()
                .Throw<XunitException>();
        }
        
        [Fact]
        public async Task FailsTwice()
        {
            var handler = new TestHandler<string>();
            await handler.As<IHandle<string>>().Handle("other");
            await handler.As<IHandle<string>>().Handle("different");
            
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
            var handler = new TestHandler<string>();
            await handler.As<IHandle<string>>().Handle("first");
            await handler.As<IHandle<string>>().Handle("then");

            handler.Assert(y => y.Should().Be("then"), TimeSpan.Zero);
        }
    }
}