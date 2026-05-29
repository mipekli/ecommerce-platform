using BuildingBlocks.Shared;

namespace Product.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public decimal Price { get; private set; }
    public string Currency { get; private set; } = "TRY";
    public string ImageUrl { get; private set; } = null!;
    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; } = null!;
    public string SKU { get; private set; } = null!;
    public bool IsPublished { get; private set; }

    private Product() { }

    public Product(string name, string description, decimal price, string imageUrl, Guid categoryId, string sku)
    {
        Name = name;
        Description = description;
        Price = price;
        ImageUrl = imageUrl;
        CategoryId = categoryId;
        SKU = sku;
    }

    public void UpdateDetails(string name, string description, decimal price, string imageUrl, Guid categoryId)
    {
        Name = name;
        Description = description;
        Price = price;
        ImageUrl = imageUrl;
        CategoryId = categoryId;
        MarkAsUpdated();
    }

    public void Publish()
    {
        IsPublished = true;
        MarkAsUpdated();
    }

    public void Unpublish()
    {
        IsPublished = false;
        MarkAsUpdated();
    }

    public void UpdatePrice(decimal newPrice)
    {
        Price = newPrice;
        MarkAsUpdated();
    }
}
