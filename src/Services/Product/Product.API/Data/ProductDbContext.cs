using Microsoft.EntityFrameworkCore;
using Product.API.Entities;

namespace Product.API.Data;

public class ProductDbContext : DbContext
{
    public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options) { }

    public DbSet<Product.API.Entities.Product> Products => Set<Product.API.Entities.Product>();
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product.API.Entities.Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.SKU).IsUnique();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Currency).IsRequired().HasMaxLength(3);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.SKU).IsRequired().HasMaxLength(100);
            entity.HasOne(e => e.Category)
                  .WithMany(c => c.Products)
                  .HasForeignKey(e => e.CategoryId);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.HasOne(e => e.ParentCategory)
                  .WithMany()
                  .HasForeignKey(e => e.ParentCategoryId)
                  .OnDelete(DeleteBehavior.NoAction);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        modelBuilder.Entity<Category>().HasData(
            new Category("Elektronik", "Elektronik ürünler") { Id = Guid.Parse("10000000-0000-0000-0000-000000000001") },
            new Category("Giyim", "Giyim ürünleri") { Id = Guid.Parse("10000000-0000-0000-0000-000000000002") },
            new Category("Kitap", "Kitap ürünleri") { Id = Guid.Parse("10000000-0000-0000-0000-000000000003") },
            new Category("Ev & Yaşam", "Ev ve yaşam ürünleri") { Id = Guid.Parse("10000000-0000-0000-0000-000000000004") },
            new Category("Spor", "Spor ürünleri") { Id = Guid.Parse("10000000-0000-0000-0000-000000000005") }
        );
    }
}
