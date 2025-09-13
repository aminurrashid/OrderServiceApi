using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OrderService.Application.DTOs;
using OrderService.Application.Factories;
using OrderService.Domain.Order;
using OrderService.Domain.Product;
using OrderService.Infrastructure.Persistence;
using OrderService.Infrastructure.Repositories;
using Xunit;

namespace Test.Infrastructure
{
    public class OrderRepositoryTests
    {
        private OrderDbContext CreateDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<OrderDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new OrderDbContext(options);
        }

        private Order CreateOrder()
        {
            var product = new Product("1", "Product 1", 10.0m, 100);
            var products = new List<Product> { product };
            var productDtos = new List<OrderItemRequestDto>
        {
            new OrderItemRequestDto(product.Id, product.Name, 2, product.Price)
        };

            return OrderFactory.Create(
                invoiceAddress: "123 Main St",
                invoiceEmailAddress: "test@example.com",
                invoiceCreditCardNumber: "4111111111111111",
                items: productDtos,
                products: products
            );
        }

        [Fact]
        public async Task AddAsync_Should_Add_Order()
        {
            var dbContext = CreateDbContext(nameof(AddAsync_Should_Add_Order));
            var repository = new OrderRepository(dbContext);
            var order = CreateOrder();

            await repository.AddAsync(order, CancellationToken.None);

            var exists = await dbContext.Orders.AnyAsync(o => o.Id == order.Id);
            exists.Should().BeTrue();
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Order_When_Exists()
        {
            var dbContext = CreateDbContext(nameof(GetByIdAsync_Should_Return_Order_When_Exists));
            var repository = new OrderRepository(dbContext);
            var order = CreateOrder();
            dbContext.Orders.Add(order);
            await dbContext.SaveChangesAsync();

            var result = await repository.GetByIdAsync(order.Id, CancellationToken.None);

            result.Should().NotBeNull();
            result!.Id.Should().Be(order.Id);
            result.Items.Should().NotBeNull();
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Null_When_Not_Exists()
        {
            var dbContext = CreateDbContext(nameof(GetByIdAsync_Should_Return_Null_When_Not_Exists));
            var repository = new OrderRepository(dbContext);

            var result = await repository.GetByIdAsync("non-existent-id", CancellationToken.None);

            result.Should().BeNull();
        }
    }
}