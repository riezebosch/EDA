using System.Threading.Tasks;
using EDA.Example;
using EDA.Example.Commands;
using EDA.Example.Events;
using EDA.Example.Services;
using EDA.TestAdapters;
using FluentAssertions;
using Xunit;
using MassTransit;
using Microsoft.Extensions.Configuration;

namespace EDA.MassTransit.IntegrationTests
{
    public class Tests
    {
        private readonly IConfigurationRoot _config;

        public Tests() =>
            _config = new ConfigurationBuilder()
                .AddUserSecrets<Tests>()
                .Build();

        [Fact]
        public async Task Test1()
        {
            var test = new Subscriber<OrderCreated>();
            var bus = Bus.Factory.CreateUsingAzureServiceBus(cfg =>
            {
                cfg.Host(_config["Azure:ServiceBus:ConnectionString"]);
                cfg.ReceiveEndpoint("service-a", x => x.SubscriberFor<CreateOrder>(c => new ServiceA(new Publisher(c))));
                cfg.ReceiveEndpoint("test", x => x.SubscriberFor<OrderCreated>(c => test));
            });
            
            await bus.StartAsync();

            var order = Generator.NewOrder();
            await bus.Publish(order);
            
            await test.Assert(x => x.Should().BeEquivalentTo(order));
        }
    }
}