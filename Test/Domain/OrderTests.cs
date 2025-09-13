using OrderService.Application.DTOs;
using OrderService.Domain.Order;
using OrderService.Domain.Product;
using OrderService.Domain.Shared;
using FluentAssertions;
using Xunit;
using OrderService.Application.Factories;

namespace Test.Domain
{
    public class OrderTests
    {
        [Fact]
        public void CreatingOrder_WithValidItems_SetsPropertiesCorrectly()
        {
            var product1 = new Product("1", "Product A", 10.99m, 100);
            var product2 = new Product("2", "Product B", 20.49m, 50);

            var productDtos = new List<OrderItemRequestDto>
            {
                new OrderItemRequestDto(product1.Id, product1.Name, 1, product1.Price),
                new OrderItemRequestDto(product2.Id, product2.Name, 1, product2.Price)
            };

            var products = new List<Product> { product1, product2 };

            var order = OrderFactory.Create(
                invoiceAddress: "123 Mock Street",
                invoiceEmailAddress: "mock@example.com",
                invoiceCreditCardNumber: "1234-5678-9012-3456",
                items: productDtos,
                products: products
            );

            order.Id.Should().NotBeNull();
            order.Items.Should().HaveCount(2);
            order.Items.Should().Contain(i => i.ProductId == product1.Id);
        }

        [Fact]
        public void CreatingOrder_WithNoItems_ProducesEmptyItemsCollection()
        {
            var product1 = new Product("1", "Product A", 10.99m, 100);
            var product2 = new Product("2", "Product B", 20.49m, 50);
            var products = new List<Product> { product1, product2 };

            var order = OrderFactory.Create("addr", "a@b.com", "cc", new List<OrderItemRequestDto>(), products);

            order.Items.Should().BeEmpty();
        }

        [Fact]
        public void AddProduct_ShouldIncreaseQuantity_WhenProductAlreadyExists()
        {
            var product = new Product("1", "Product A", 10.99m, 100);
            var products = new List<Product> { product };
            var productDtos = new List<OrderItemRequestDto>
            {
                new OrderItemRequestDto(product.Id, product.Name, 1, product.Price)
            };
            var order = OrderFactory.Create("addr", "a@b.com", "cc", productDtos, products);

            order.AddProduct(product, 2);

            var item = order.Items.Single(i => i.ProductId == product.Id);
            item.Quantity.Should().Be(3);
        }
        
        [Fact]
        public void AddProduct_ShouldThrow_WhenProductAmountInvalid()
        {
            var product = new Product("1", "Product A", 10.99m, 100);
            var products = new List<Product> { product };
            var productDtos = new List<OrderItemRequestDto>
            {
                new OrderItemRequestDto(product.Id, product.Name, 1, product.Price)
            };
            var order = OrderFactory.Create("addr", "a@b.com", "cc", productDtos, products);

            FluentActions.Invoking(() => order.AddProduct(product, 0))
                .Should().Throw<DomainException>().WithMessage("Added quantity must be positive.");
        }

        [Fact]
        public void AddProduct_ShouldThrow_WhenProductIsNull()
        {
            var product = new Product("1", "Product A", 10.99m, 100);
            var products = new List<Product> { product };
            var order = OrderFactory.Create("addr", "a@b.com", "cc", new List<OrderItemRequestDto>(), products);

            FluentActions.Invoking(() => order.AddProduct(null, 1))
                .Should().Throw<DomainException>().WithMessage("Product cannot be null.");
        }

        [Fact]
        public void AddProduct_ShouldThrow_WhenNotEnoughStock()
        {
            var product = new Product("1", "Product A", 10.99m, 1);
            var products = new List<Product> { product };
            var order = OrderFactory.Create("addr", "a@b.com", "cc", new List<OrderItemRequestDto>(), products);

            FluentActions.Invoking(() => order.AddProduct(product, 2))
                .Should().Throw<DomainException>().WithMessage($"Not enough stock for product {product.Id}. Requested 2, available 1.");
        }

        [Fact]
        public void CreatingOrder_WithInvalidEmail_ThrowsDomainException()
        {
            var product = new Product("1", "Product A", 10.99m, 100);
            var products = new List<Product> { product };
            var productDtos = new List<OrderItemRequestDto>
            {
                new OrderItemRequestDto(product.Id, product.Name, 1, product.Price)
            };

            FluentActions.Invoking(() =>
                OrderFactory.Create("addr", "invalid-email", "cc", productDtos, products))
                .Should().Throw<DomainException>().WithMessage("Invalid customer email.");
        }

        [Fact]
        public void CreatingOrder_WithNullAddress_ThrowsDomainException()
        {
            var product = new Product("1", "Product A", 10.99m, 100);
            var products = new List<Product> { product };
            var productDtos = new List<OrderItemRequestDto>
            {
                new OrderItemRequestDto(product.Id, product.Name, 1, product.Price)
            };

            FluentActions.Invoking(() =>
                OrderFactory.Create(null, "a@b.com", "cc", productDtos, products))
                .Should().Throw<DomainException>().WithMessage("Shipping address is required.");
        }
    }
}