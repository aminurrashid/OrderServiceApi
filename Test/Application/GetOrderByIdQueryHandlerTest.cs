using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using OrderService.Application.DTOs;
using OrderService.Application.Queries.GetOrderById;
using OrderService.Domain.Order;
using OrderService.Domain.Product;
using OrderService.Infrastructure.Repositories;
using Xunit;

namespace Test.Application
{
    public class GetOrderByIdQueryHandlerTests
    {
        private readonly Mock<IOrderRepository> _orderRepoMock = new();
        private readonly Mock<IValidator<GetOrderByIdQuery>> _validatorMock = new();

        private GetOrderByIdQueryHandler CreateHandler() =>
            new GetOrderByIdQueryHandler(_orderRepoMock.Object, _validatorMock.Object);

        private GetOrderByIdQuery GetValidQuery(string orderNumber) =>
            new GetOrderByIdQuery(orderNumber);

        private OrderDto GetFakeOrderDto()
        {
            var items = new List<OrderItemReponseDto>
        {
            new("item-1", "p1", "Product 1", 2, 10.0m),
            new("item-2", "p2", "Product 2", 1, 20.0m)
        };
            return new OrderDto(
                "order-123",
                "123 Main St",
                "customer@example.com",
                "4111111111111111",
                DateTime.UtcNow,
                items
            );

        }
        private Order GetFakeOrder()
        {
            var items = new List<OrderItemRequestDto>
        {
            new( "p1", "Product 1", 2, 10.0m),
            new( "p2", "Product 2", 1, 20.0m)
        };
            var products = new List<Product>
        {
            new("p1", "Product 1", 10.0m, 100),
            new("p2", "Product 2", 20.0m, 50)
        };
            return new Order(
                "123 Main St",
                "customer@example.com",
                "4111111111111111",
                items,
                products
            );

        }


        [Fact]
        public async Task Handle_Should_Return_OrderDto_When_OrderExists()
        {
            var order = GetFakeOrder();
            var handler = CreateHandler();
            var query = GetValidQuery(order.Id);

            _validatorMock
                .Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());


            _orderRepoMock
                .Setup(r => r.GetByIdAsync(order.Id))
                .ReturnsAsync(order);

            var result = await handler.Handle(query, CancellationToken.None);

            result.Should().NotBeNull();
            result!.OrderNumber.Should().Be(order.Id);
            result.OrderItems.Should().HaveCount(2);
            result.InvoiceAddress.Should().Be("123 Main St");
        }

        [Fact]
        public async Task Handle_Should_ThrowValidationException_When_ValidationFails()
        {
            var query = GetValidQuery("some-order-number");
            var handler = CreateHandler();

            _validatorMock
                .Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(new[] {
                new ValidationFailure("OrderNumber", "Invalid order number")
                }));

            var act = async () => await handler.Handle(query, CancellationToken.None);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task Handle_Should_ReturnNull_When_OrderNotFound()
        {
            var query = GetValidQuery("some-order-number");
            var handler = CreateHandler();

            _validatorMock
                .Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _orderRepoMock
                .Setup(r => r.GetByIdAsync("order-123"))
                .ReturnsAsync((Order?)null);

            var result = await handler.Handle(query, CancellationToken.None);

            result.Should().BeNull();
        }
    }
}
