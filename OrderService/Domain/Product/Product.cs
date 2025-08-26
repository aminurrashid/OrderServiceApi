namespace OrderService.Domain.Product;

public class Product
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    
    public int AvailableStock { get; private set; }

    private Product() { }

    public Product(string id, string name, decimal price, int availableStock)
    {
        Id = id;
        Name = name;
        Price = price;
        AvailableStock = availableStock;
    }
}