using OrderService.Application.DTOs;
using OrderService.Domain.Order;
using OrderService.Domain.Product;

namespace OrderService.Application.Factories
{
    public static class OrderFactory
    {
        public static Order Create(string invoiceAddress, string invoiceEmailAddress, string invoiceCreditCardNumber, List<OrderItemRequestDto> items, List<Product> products)
        {
            var order = new Order(invoiceAddress, invoiceEmailAddress, invoiceCreditCardNumber, products);
            items
                .Select(i => new { Item = i, Product = products.Single(p => p.Id.ToString() == i.ProductId) })
                .ToList()
                .ForEach(x => order.AddProduct(x.Product, x.Item.ProductAmount));
            return order;
        }
    }
}
