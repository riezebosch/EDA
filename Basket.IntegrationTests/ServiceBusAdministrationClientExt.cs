using System.Threading.Tasks;
using Azure.Messaging.ServiceBus.Administration;

namespace Basket.IntegrationTests
{
    internal static class ServiceBusAdministrationClientExt
    {
        public static async Task Setup(this ServiceBusAdministrationClient admin, string topic, string subscription, string @event)
        {
            await SetupSubscription(admin, topic, subscription);
            await DeleteDefaultRule(admin, topic, subscription);
            await SetupRule(admin, topic, subscription, @event);
        }

        private static async Task DeleteDefaultRule(ServiceBusAdministrationClient admin, string topic, string subscription)
        {
            const string name = "$Default";
            if ((await admin.RuleExistsAsync(topic, subscription, name)).Value)
            {
                await admin.DeleteRuleAsync(topic, subscription, name);
            }
        }

        private static async Task SetupRule(ServiceBusAdministrationClient admin, string topic, string subscription, string @event)
        {
            if (!(await admin.RuleExistsAsync(topic, subscription, @event)).Value)
            {
                await admin.CreateRuleAsync(topic, subscription,
                    new CreateRuleOptions(@event, new CorrelationRuleFilter { Subject = @event }));
            }
        }

        private static async Task SetupSubscription(ServiceBusAdministrationClient admin, string topic, string subscription)
        {
            if (!(await admin.SubscriptionExistsAsync(topic, subscription)).Value)
            {
                await admin.CreateSubscriptionAsync(topic, subscription);
            }
        }
    }
}