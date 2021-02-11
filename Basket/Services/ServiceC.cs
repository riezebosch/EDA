using System;
using System.Threading.Tasks;
using Basket.Events;

namespace Basket.Services
{
    public class ServiceC : IHandle<OrderShipped>
    {
        private readonly IPublisher _publisher;

        public ServiceC(IPublisher publisher) => 
            _publisher = publisher;

        public Task Handle(OrderShipped body) =>
            _publisher.Publish("InvoiceSent", new InvoiceSent {Id = body.Id}, DateTimeOffset.UtcNow);
    }
}