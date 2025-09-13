using OrderService.Domain.Product;

namespace OrderService.Infrastructure.Repositories;

public interface IProductRepository
{
    Task<IEnumerable<Product>> FindByIdsAsync(List<string> productIds, CancellationToken cancellationToken);
}