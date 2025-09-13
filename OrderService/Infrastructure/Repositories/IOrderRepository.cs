using OrderService.Domain.Order;

namespace OrderService.Infrastructure.Repositories
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order, CancellationToken cancellationToken);
        Task<Order?> GetByIdAsync(string orderNumber, CancellationToken cancellationToken);
    }
}
