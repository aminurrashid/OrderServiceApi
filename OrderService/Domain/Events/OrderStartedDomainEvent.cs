using MediatR;

namespace OrderService.Domain.Events
{
    public record OrderStartedDomainEvent(
        Order.Order Ordert) : INotification;
}
