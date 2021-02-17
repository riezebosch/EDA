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

        public async Task Publish(string @event, object body, DateTimeOffset schedule)
        {
            await _sender.SendMessageAsync(new ServiceBusMessage(JsonSerializer.Serialize(body))
            {
                Subject = @event, 
                ScheduledEnqueueTime = schedule
            });
        }
    }
}