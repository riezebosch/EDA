using System;
using System.Linq;
using System.Threading.Tasks;
using Basket.Commands;
using Basket.Events;
using Basket.Services;
using NSubstitute;
using Xunit;

namespace Basket.Tests
{
    public class ServiceATests
    {
        [Fact]
        public async Task Publish()
        {
            // arrange
            var order = new CreateOrder
            {
                Id = 5,
                Email = "someone@somewhere.tld",
                Items = new []
                {
                    new CreateOrder.Item { Name = "Some item", Price = 2.5m, Quantity = 3 },
                    new CreateOrder.Item { Name = "Other item", Price = 5m, Quantity = 2 }
                }
            };
            var publisher = Substitute.For<IPublisher>();

            // act
            await new ServiceA(publisher).Handle(order);
            
            // assert
            await publisher
                .Received()
                .Publish("OrderCreated", Arg.Is<OrderCreated>(x => x.Id == 5 && x.Items.Count() == 2 && x.TotalAmount == 17.5m), Arg.Any<DateTimeOffset>());
        }
    }
}