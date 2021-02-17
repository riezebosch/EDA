using System.Collections.Generic;

namespace EDA.Example.Events
{
    public class OrderCreated
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public IEnumerable<Item> Items { get; set; }
        public decimal TotalAmount { get; set; }
        
        public class Item
        {
            public string Name { get; set; }
            public decimal Price { get; set; }
            public int Quantity { get; set; }
        }
    }
}