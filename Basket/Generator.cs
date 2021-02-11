using Basket.Commands;
using Basket.Events;
using Bogus;

namespace Basket
{
    public static class Generator
    {
        public static CreateOrder NewOrder()
        {
            var item = new Faker<CreateOrder.Item>()
                .StrictMode(true)
                .RuleFor(i => i.Name, f => f.Commerce.Product())
                .RuleFor(i => i.Quantity, f => f.Random.Number(10))
                .RuleFor(i => i.Price, f => 0.01m * f.Random.Number(10, 3000));

            var order = new Faker<CreateOrder>()
                .StrictMode(true)
                .RuleFor(x => x.Id, f => f.UniqueIndex)
                .RuleFor(x => x.Email, f => f.Internet.Email())
                .RuleFor(x => x.Items, f => item.Generate(5));

            return order.Generate();
        }
    }
}