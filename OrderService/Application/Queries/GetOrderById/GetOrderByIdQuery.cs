using MediatR;
using OrderService.Application.DTOs;

namespace OrderService.Application.Queries.GetOrderById
{
    public record GetOrderByIdQuery(string OrderNumber) : IRequest<OrderDto?>;
}
