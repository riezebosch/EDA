using System;
using System.Threading.Tasks;
using EDA.Example.Events;
using EDA.Ports;

namespace EDA.Example.Services
{
    public class ServiceB : ISubscribe<OrderCreated>
    {
        private readonly IPublisher _publisher;

        public ServiceB(IPublisher publisher) => 
            _publisher = publisher;

        public Task Handle(OrderCreated body) => 
            _publisher.Publish("OrderShipped", new OrderShipped { Id = body.Id }, DateTimeOffset.UtcNow.AddSeconds(5));
    }
}