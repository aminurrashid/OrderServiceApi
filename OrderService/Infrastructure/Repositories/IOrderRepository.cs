using OrderService.Domain.Order;

namespace OrderService.Infrastructure.Repositories
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order);
        Task<Order?> GetByIdAsync(string orderNumber);
    }
}
