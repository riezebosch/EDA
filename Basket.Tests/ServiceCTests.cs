using System;
using System.Threading.Tasks;
using Basket.Events;
using Basket.Services;
using NSubstitute;
using Xunit;

namespace Basket.Tests
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