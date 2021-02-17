using System;
using System.Threading.Tasks;
using EDA.Example.Events;
using EDA.Example.Services;
using EDA.Ports;
using NSubstitute;
using Xunit;

namespace EDA.Example.Tests
{
    public class ServiceCTests
    {
        [Fact]
        public async Task Publish()
        {
            // arrange
            var publisher = Substitute.For<IPublisher>();

            // act
            await new ServiceC(publisher).Handle(new OrderShipped());
            
            // assert
            await publisher
                .Received()
                .Publish("InvoiceSent", Arg.Any<InvoiceSent>(), Arg.Any<DateTimeOffset>());
        }
    }
}