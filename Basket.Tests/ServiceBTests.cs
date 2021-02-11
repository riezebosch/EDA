using System;
using System.Threading.Tasks;
using Basket.Events;
using Basket.Services;
using NSubstitute;
using Xunit;

namespace Basket.Tests
{
    public class ServiceBTests
    {
        [Fact]
        public async Task Publish()
        {
            // arrange
            var publisher = Substitute.For<IPublisher>();

            // act
            await new ServiceB(publisher).Handle(new OrderCreated { Id = 5});
            
            // assert
            await publisher
                .Received()
                .Publish("OrderShipped", Arg.Is<OrderShipped>(x => x.Id == 5), Arg.Any<DateTimeOffset>());
        }
    }
}