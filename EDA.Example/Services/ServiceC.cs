using System;
using System.Threading.Tasks;
using EDA.Example.Events;
using EDA.Ports;

namespace EDA.Example.Services
{
    public class ServiceC : ISubscribe<OrderShipped>
    {
        private readonly IPublisher _publisher;

        public ServiceC(IPublisher publisher) => 
            _publisher = publisher;

        public Task Handle(OrderShipped body) =>
            _publisher.Publish("InvoiceSent", new InvoiceSent {Id = body.Id}, DateTimeOffset.UtcNow);
    }
}