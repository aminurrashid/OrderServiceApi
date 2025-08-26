using FluentValidation;

namespace OrderService.Application.Queries.GetOrderById
{
    public class GetOrderByIdQueryValidator : AbstractValidator<GetOrderByIdQuery>
    {
        public GetOrderByIdQueryValidator()
        {
            RuleFor(x => x.OrderNumber)
                .NotEmpty().WithMessage("Order number must be provided.");

            RuleFor(x => x.OrderNumber)
                .Must(v => Guid.TryParse(v, out _))
                .WithMessage("Order number must be a valid GUID.");
        }
    }
}
