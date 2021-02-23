using System;
using EDA.Ports;
using MassTransit;

namespace EDA.MassTransit
{
    public static class Factory
    {
        public static void SubscriberFor<T>(this IReceiveEndpointConfigurator x, Func<ConsumeContext<T>, ISubscribe<T>> service) where T: class => 
            x.Instance(new Consumer<T>(service));
    }
}