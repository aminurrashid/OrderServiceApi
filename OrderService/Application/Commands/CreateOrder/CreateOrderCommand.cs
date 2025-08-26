using MediatR;
using OrderService.Application.DTOs;

namespace OrderService.Application.Commands.CreateOrder
{
    public record CreateOrderCommand(
        string InvoiceAddress,
        string InvoiceEmailAddress,
        string InvoiceCreditCardNumber,
        List<OrderItemRequestDto> OrderItems
    ) : IRequest<string>;

}
  