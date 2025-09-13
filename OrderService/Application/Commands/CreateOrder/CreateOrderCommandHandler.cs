using FluentValidation;
using MediatR;
using OrderService.Application.DTOs;
using OrderService.Application.Factories;
using OrderService.Domain.Order;
using OrderService.Domain.Product;
using OrderService.Domain.Shared;
using OrderService.Infrastructure.Middleware;
using OrderService.Infrastructure.Repositories;
using Serilog;

namespace OrderService.Application.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, string>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IValidator<CreateOrderCommand> _validator;
        private readonly ILogger<CreateOrderCommandHandler> _logger;

        public CreateOrderCommandHandler
            (IOrderRepository orderRepository, 
            IProductRepository productRepository, 
            IValidator<CreateOrderCommand> validator, 
            ILogger<CreateOrderCommandHandler> logger)
        {
            _validator = validator;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<string> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            List<OrderItemRequestDto> productDtos = request.OrderItems;
            var productIds = productDtos.Select(i => i.ProductId).ToList();
            IEnumerable<Product> products = await _productRepository.FindByIdsAsync(productIds, cancellationToken);
            if (products.Count() != productIds.Count)
                throw new DomainException("One or more products not found.");

            var order = OrderFactory.Create(request.InvoiceAddress, request.InvoiceEmailAddress, request.InvoiceCreditCardNumber, productDtos, products.ToList());

            await _orderRepository.AddAsync(order, cancellationToken);
            _logger.LogInformation("Order created with ID: {OrderId}", order.Id);
            return order.Id;
        }
    }
}
