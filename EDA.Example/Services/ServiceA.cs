using System;
using System.Linq;
using System.Threading.Tasks;
using EDA.Example.Commands;
using EDA.Example.Events;
using EDA.Ports;

namespace EDA.Example.Services
{
    public class ServiceA : ISubscribe<CreateOrder>
    {
        private readonly IPublisher _publisher;

        public ServiceA(IPublisher publisher) => 
            _publisher = publisher;

        public async Task Handle(CreateOrder body) => 
            await _publisher.Publish("OrderCreated", ToEvent(body), DateTimeOffset.UtcNow.AddSeconds(5));

        private static OrderCreated ToEvent(CreateOrder order) => 
            new()
            {
                Id = order.Id, 
                Email = order.Email,
                Items = order.Items.Select(x => ToEvent(x)), 
                TotalAmount = order.Items.Select(x => x.Price * x.Quantity).Sum()
            };

        private static OrderCreated.Item ToEvent(CreateOrder.Item item) => 
            new()
            {
                Name = item.Name,
                Price = item.Price, 
                Quantity = item.Quantity
            };
    }
}