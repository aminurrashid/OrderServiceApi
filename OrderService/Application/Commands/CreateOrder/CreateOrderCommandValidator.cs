using FluentValidation;

namespace OrderService.Application.Commands.CreateOrder
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(x => x.InvoiceAddress)
                .NotEmpty().WithMessage("Invoice address is required.");

            RuleFor(x => x.InvoiceEmailAddress)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Must be a valid email.");

            RuleFor(x => x.InvoiceCreditCardNumber)
                .NotEmpty().WithMessage("Credit card number is required.")
                .CreditCard().WithMessage("Invalid credit card number.");

            RuleFor(x => x.OrderItems)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Order items must be provided.")
                .NotEmpty().WithMessage("At least one product is required.");

            RuleForEach(x => x.OrderItems).ChildRules(prod =>
            {
                prod.RuleFor(p => p.ProductId)
                    .NotEmpty().WithMessage("ProductId is required.");

                prod.RuleFor(p => p.ProductName)
                    .NotEmpty().WithMessage("ProductName is required.");

                prod.RuleFor(p => p.ProductAmount)
                    .GreaterThan(0).WithMessage("Amount must be at least 1.");

                prod.RuleFor(p => p.ProductPrice)
                    .GreaterThan(0).WithMessage("Price must be positive.");
            });
        }
    }
}
