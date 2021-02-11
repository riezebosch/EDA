using System;
using System.Threading.Tasks;

namespace Basket
{
    public interface IPublisher
    {
        Task Publish(string @event, object body, DateTimeOffset schedule);
    }
}