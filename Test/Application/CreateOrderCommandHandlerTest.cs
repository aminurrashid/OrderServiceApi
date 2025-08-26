using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using OrderService.Application.Commands.CreateOrder;
using OrderService.Application.DTOs;
using OrderService.Domain.Order;
using OrderService.Domain.Product;
using OrderService.Domain.Shared;
using OrderService.Infrastructure.Repositories;
using Xunit;

namespace Test.Application
{
    public class CreateOrderCommandHandlerTests
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock = new();
        private readonly Mock<IProductRepository> _productRepositoryMock = new();
        private readonly Mock<IValidator<CreateOrderCommand>> _validatorMock = new();

        private CreateOrderCommandHandler CreateHandler() =>
            new CreateOrderCommandHandler(_orderRepositoryMock.Object, _productRepositoryMock.Object, _validatorMock.Object);

        private CreateOrderCommand GetValidCommand(List<OrderItemRequestDto> orderItems)
        {
            return new CreateOrderCommand
            (
                "123 Main St",
                "customer@example.com",
                "4111111111111111",
                orderItems
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnOrderId_WhenCommandIsValid()
        {
            var command = GetValidCommand(new List<OrderItemRequestDto>
            {
                new OrderItemRequestDto("prod-1", "", 2, 10.0m),
            });
            var product = new Product("prod-1", "Product 1", 12m, 12);
            var handler = CreateHandler();

            _validatorMock
                .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _productRepositoryMock
                .Setup(repo => repo.FindByIdsAsync(It.IsAny<List<string>>()))
                .ReturnsAsync(new List<Product> { product });

            _orderRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<Order>()))
                .Returns(Task.CompletedTask)
                ;


            var result = await handler.Handle(command, CancellationToken.None);

            result.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Handle_ShouldThrowDomainException_WhenOrderItemsEmpty()
        {
            var command = GetValidCommand(new List<OrderItemRequestDto>());
            var handler = CreateHandler();

            _validatorMock
                .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ValidationException("At least one product is required."));

            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("At least one product is required.");
        }

        [Fact]
        public async Task Handle_ShouldThrowDomainException_WhenProductNotFound()
        {
            var command = GetValidCommand(new List<OrderItemRequestDto>
            {
                new OrderItemRequestDto("prod-1", "", 2, 10.0m),
            });
            var handler = CreateHandler();

            _validatorMock
                .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _productRepositoryMock
                .Setup(repo => repo.FindByIdsAsync(It.IsAny<List<string>>()))
                .ReturnsAsync(new List<Product>());

            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("One or more products not found.");
        }
    }
}