using FluentAssertions;
using OrderService.Application.Commands.CreateOrder;
using OrderService.Application.DTOs;
using System.Collections.Generic;
using Xunit;

namespace Test.Application
{
    public class CreateOrderCommandValidatorTests
    {
        private readonly CreateOrderCommandValidator _validator = new();

        private CreateOrderCommand GetValidCommand() =>
            new CreateOrderCommand(
                "123 Main St",
                "test@example.com",
                "4111111111111111",
                new List<OrderItemRequestDto>
                {
                new OrderItemRequestDto(
                    "p1",
                    "Product 1",
                    2,
                    10.0m
                )
                }
            );

        [Fact]
        public void Should_Pass_When_Command_Is_Valid()
        {
            var command = GetValidCommand();

            var result = _validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Should_Fail_When_Email_Is_Invalid()
        {
            var command = GetValidCommand() with { InvoiceEmailAddress = "not-an-email" };

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(command.InvoiceEmailAddress));
        }

        [Fact]
        public void Should_Fail_When_CreditCard_Is_Invalid()
        {
            var command = GetValidCommand() with { InvoiceCreditCardNumber = "123" };

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(command.InvoiceCreditCardNumber));
        }

        [Fact]
        public void Should_Fail_When_OrderItems_Is_Empty()
        {
            var command = GetValidCommand() with { OrderItems = new List<OrderItemRequestDto>() };

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(command.OrderItems));
        }

        [Fact]
        public void Should_Fail_When_ProductAmount_Is_Zero()
        {
            var invalidItem = new OrderItemRequestDto("p1", "Product 1", 0, 10.0m);
            var command = GetValidCommand() with { OrderItems = new List<OrderItemRequestDto> { invalidItem } };

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName.Contains("ProductAmount"));
        }
    }
}