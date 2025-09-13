
using OrderService.Domain.Events;
using OrderService.Domain.Shared;

namespace OrderService.Domain.Order
{
    public class Order : Entity
    {
        public string Id { get; private set; }
        public string InvoiceAddress { get; private set; }
        public string InvoiceEmailAddress { get; private set; }
        public string InvoiceCreditCardNumber { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private readonly List<OrderItem> _items = new();
        public IReadOnlyCollection<OrderItem> Items => _items;

        private Order() { }
        
        public Order(string address, string email, string creditCard, IEnumerable<Product.Product> products)
        {
            Id = Guid.NewGuid().ToString();
            SetShippingAddress(address);
            SetCustomerEmail(email);
            SetCreditCard(creditCard);
            InvoiceCreditCardNumber = creditCard;
            CreatedAt = DateTime.UtcNow;
            AddDomainEvent(new OrderStartedDomainEvent(this));
        }

        public void AddProduct(Product.Product product, int quantity)
        {
            if (product == null)
                throw new DomainException("Product cannot be null.");
            if (quantity > product.AvailableStock)
                throw new DomainException(
                    $"Not enough stock for product {product.Id}. Requested {quantity}, available {product.AvailableStock}.");

            var existing = _items.SingleOrDefault(i => i.ProductId == product.Id);
            if (existing != null)
            {
                existing.IncreaseQuantity(quantity);
            }
            else
            {
                var item = new OrderItem(product.Id, product.Name, product.Price, quantity);
                _items.Add(item);
            }
        }
        
        private void SetCreditCard(string creditCardNumber)
        {
            InvoiceCreditCardNumber = creditCardNumber;
        }

        private void SetShippingAddress(string address)
        {
            InvoiceAddress = address ?? throw new DomainException("Shipping address is required.");
        }

        private void SetCustomerEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                throw new DomainException("Invalid customer email.");
            InvoiceEmailAddress = email;
        }
    }
}
