using System;
using System.Text.Json;
using Azure.Messaging.ServiceBus;

namespace EDA.ServiceBus
{
    internal static class Event
    {
        public static ServiceBusMessage ToEvent(this object body, string @event, DateTimeOffset schedule) =>
            new(JsonSerializer.Serialize(body))
            {
                Subject = @event, 
                ScheduledEnqueueTime = schedule
            };

        public static T FromEvent<T>(this ServiceBusReceivedMessage message) => 
            JsonSerializer.Deserialize<T>(message.Body);
    }
}