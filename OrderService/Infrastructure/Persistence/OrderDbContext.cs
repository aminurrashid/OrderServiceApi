using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Order;

namespace OrderService.Infrastructure.Persistence
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>(o =>
            {
                o.HasKey(x => x.Id);
                o.HasIndex(x => x.Id);
                o.HasIndex(x => x.InvoiceEmailAddress);
                o.Property(x => x.InvoiceEmailAddress).HasMaxLength(150);
                o.Property(x => x.InvoiceAddress).HasMaxLength(300);
                o.Property(x => x.InvoiceCreditCardNumber).HasMaxLength(20);
                o.OwnsMany(x => x.Items, oi =>
                {
                    oi.WithOwner().HasForeignKey("OrderId");
                    oi.HasKey(x => x.Id);
                    oi.HasIndex("OrderId");
                    oi.Property(x => x.Name).HasMaxLength(200);
                    oi.Property(x => x.ProductId).HasMaxLength(200);
                });
            });
        }
    }
}
