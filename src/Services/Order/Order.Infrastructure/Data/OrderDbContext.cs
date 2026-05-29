using Microsoft.EntityFrameworkCore;
using Order.Domain.Entities;
using Order.Domain.ValueObjects;

namespace Order.Infrastructure.Data;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

    public DbSet<Order.Domain.Entities.Order> Orders => Set<Order.Domain.Entities.Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order.Domain.Entities.Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.OrderNumber).IsUnique();
            entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.SubTotal).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TaxAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.ShippingCost).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Notes).HasMaxLength(500);

            entity.OwnsOne(e => e.ShippingAddress, a =>
            {
                a.Property(p => p.Street).HasColumnName("ShippingStreet").HasMaxLength(200);
                a.Property(p => p.City).HasColumnName("ShippingCity").HasMaxLength(100);
                a.Property(p => p.State).HasColumnName("ShippingState").HasMaxLength(100);
                a.Property(p => p.ZipCode).HasColumnName("ShippingZipCode").HasMaxLength(20);
                a.Property(p => p.Country).HasColumnName("ShippingCountry").HasMaxLength(100);
            });

            entity.OwnsOne(e => e.BillingAddress, a =>
            {
                a.Property(p => p.Street).HasColumnName("BillingStreet").HasMaxLength(200);
                a.Property(p => p.City).HasColumnName("BillingCity").HasMaxLength(100);
                a.Property(p => p.State).HasColumnName("BillingState").HasMaxLength(100);
                a.Property(p => p.ZipCode).HasColumnName("BillingZipCode").HasMaxLength(20);
                a.Property(p => p.Country).HasColumnName("BillingCountry").HasMaxLength(100);
            });

            entity.HasMany(e => e.Items)
                  .WithOne(e => e.Order)
                  .HasForeignKey(e => e.OrderId);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProductName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
            entity.HasQueryFilter(e => !e.IsDeleted);
        });
    }
}
