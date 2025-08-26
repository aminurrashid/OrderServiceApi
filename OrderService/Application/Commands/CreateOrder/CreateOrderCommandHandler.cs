using FluentValidation;
using MediatR;
using OrderService.Application.DTOs;
using OrderService.Domain.Order;
using OrderService.Domain.Product;
using OrderService.Domain.Shared;
using OrderService.Infrastructure.Repositories;
using Serilog;

namespace OrderService.Application.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, string>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IValidator<CreateOrderCommand> _validator;

        public CreateOrderCommandHandler(IOrderRepository orderRepository, IProductRepository productRepository, IValidator<CreateOrderCommand> validator)
        {
            _validator = validator;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        public async Task<string> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            List<OrderItemRequestDto> productDtos = request.OrderItems;
            var productIds = productDtos.Select(i => i.ProductId).ToList();
            IEnumerable<Product> products = await _productRepository.FindByIdsAsync(productIds);
            if (products.Count() != productIds.Count)
                throw new DomainException("One or more products not found.");

            var order = new Order(request.InvoiceAddress, request.InvoiceEmailAddress, request.InvoiceCreditCardNumber
                , productDtos, products);
            
            await _orderRepository.AddAsync(order);
            Log.Information("Order created with ID: {OrderId}", order.Id);
            return order.Id;
        }
    }
}
