using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using EDA.Ports;

namespace EDA.EventHubs
{
    public class Publisher : IPublisher
    {
        private readonly EventHubProducerClient _client;

        public Publisher(EventHubProducerClient client) => 
            _client = client;

        public Task Publish(string @event, object body, DateTimeOffset schedule) =>
            _client.SendAsync(new[] { body.ToEvent(@event) });
    }
}