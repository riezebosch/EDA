using System;
using System.Threading.Tasks;
using EDA.Example.Events;
using EDA.Example.Services;
using EDA.Ports;
using NSubstitute;
using Xunit;

namespace EDA.Example.Tests
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