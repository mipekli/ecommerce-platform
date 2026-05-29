using Microsoft.EntityFrameworkCore;
using Inventory.API.Entities;

namespace Inventory.API.Data;

public class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options) { }

    public DbSet<StockItem> StockItems => Set<StockItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StockItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ProductId).IsUnique();
            entity.HasIndex(e => e.SKU).IsUnique();
            entity.Property(e => e.ProductName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.SKU).IsRequired().HasMaxLength(100);
            entity.Property(e => e.WarehouseLocation).IsRequired().HasMaxLength(100);
            entity.Property(e => e.QuantityOnHand);
            entity.Property(e => e.ReservedQuantity);
            entity.Property(e => e.LowStockThreshold);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });
    }
}
