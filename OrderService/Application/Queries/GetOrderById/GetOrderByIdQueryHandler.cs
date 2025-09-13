using FluentValidation;
using MediatR;
using OrderService.Application.DTOs;
using OrderService.Infrastructure.Repositories;

namespace OrderService.Application.Queries.GetOrderById
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto?>
    {
        private readonly IOrderRepository _repository;
        private readonly IValidator<GetOrderByIdQuery> _validator;

        public GetOrderByIdQueryHandler(IOrderRepository repository, IValidator<GetOrderByIdQuery> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var order = await _repository.GetByIdAsync(request.OrderNumber, cancellationToken);
            if (order is null) return null;

            var items = order.Items.Select(i => new OrderItemReponseDto
            (
                i.Id,
                i.ProductId,
                i.Name,
                i.Quantity,
                i.Price
            )).ToList(); 
            var dto = new OrderDto
            (
                order.Id.ToString(),
                order.InvoiceAddress,
                order.InvoiceEmailAddress,
                order.InvoiceCreditCardNumber,
                order.CreatedAt,
                items
            );

            return dto;
        }
    }
}
