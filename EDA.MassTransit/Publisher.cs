using System;
using System.Threading.Tasks;
using EDA.Ports;
using MassTransit;

namespace EDA.MassTransit
{
    public class Publisher : IPublisher
    {
        private readonly ConsumeContext _ctx;

        public Publisher(ConsumeContext ctx) => 
            _ctx = ctx;

        public Task Publish(string @event, object body, DateTimeOffset schedule) => 
            _ctx.Publish(body);
    }
}