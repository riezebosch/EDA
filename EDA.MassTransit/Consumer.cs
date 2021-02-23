using System;
using System.Threading.Tasks;
using EDA.Ports;
using MassTransit;

namespace EDA.MassTransit
{
    public class Consumer<TMessage> : IConsumer<TMessage> where TMessage: class
    {
        private readonly Func<ConsumeContext<TMessage>, ISubscribe<TMessage>> _factory;

        public Consumer(Func<ConsumeContext<TMessage>, ISubscribe<TMessage>> factory) => 
            _factory = factory;


        public Task Consume(ConsumeContext<TMessage> context) =>
            _factory(context).Handle(context.Message);
    }
}