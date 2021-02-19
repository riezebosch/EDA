using System.Text;
using System.Text.Json;
using Azure.Messaging.EventHubs;

namespace EDA.EventHubs
{
    internal static class Event
    {
        public static EventData ToEvent(this object body, string @event) =>
            new(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(body)))
            {
                Properties = {["event"] = @event}
            };

        public static T FromEvent<T>(this EventData data) => 
            JsonSerializer.Deserialize<T>(data.EventBody);
    }
}