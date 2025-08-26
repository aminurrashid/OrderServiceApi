using OrderService.Domain.Product;

namespace OrderService.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        public Task<IEnumerable<Product>> FindByIdsAsync(List<string> productIds)
        {
            var products = new List<Product>
            {
                new Product("1", "Product A", 100, 50),
                new Product("2", "Product B", 200, 30),
                new Product("3", "Product C", 300, 20)
            };

            var result = products.Where(p => productIds.Contains(p.Id));
            return Task.FromResult(result.AsEnumerable());
        }
    }
}
