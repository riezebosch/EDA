using System;
using System.Threading.Tasks;
using Basket.Events;

namespace Basket.Services
{
    public class ServiceB : IHandle<OrderCreated>
    {
        private readonly IPublisher _publisher;

        public ServiceB(IPublisher publisher) => 
            _publisher = publisher;

        public Task Handle(OrderCreated body) => 
            _publisher.Publish("OrderShipped", new OrderShipped { Id = body.Id }, DateTimeOffset.UtcNow.AddSeconds(5));
    }
}