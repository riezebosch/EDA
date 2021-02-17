using System;
using System.Threading.Tasks;

namespace EDA.Ports
{
    public interface IPublisher
    {
        Task Publish(string @event, object body, DateTimeOffset schedule);
    }
}