using System;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using EDA.Ports;

namespace EDA.ServiceBus
{
    public class Publisher : IPublisher
    {
        private readonly ServiceBusSender _sender;

        public Publisher(ServiceBusSender sender) => 
            _sender = sender;

        public Task Publish(string @event, object body, DateTimeOffset schedule) => 
            _sender.SendMessageAsync(body.ToEvent(@event, schedule));
    }
}