using OrderService.Domain.Shared;

namespace OrderService.Domain.Order
{
    public class OrderItem
    {
        public string Id { get; private set; }
        public string ProductId { get; private set; }
        public string Name { get; private set; }
        public int Quantity { get; private set; }
        public decimal Price { get; private set; }

        private OrderItem() { }
        
        internal OrderItem(string productId, string name, decimal price, int quantity)
        {
            if (quantity <= 0)
                throw new DomainException("Quantity must be at least 1.");
            
            Id = Guid.NewGuid().ToString();
            ProductId = productId;
            Name = name;
            Price   = price;
            Quantity    = quantity;
        }

        internal void IncreaseQuantity(int add)
        {
            if (add <= 0)
                throw new DomainException("Added quantity must be positive.");
            Quantity += add;
        }
    }
}
