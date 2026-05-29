using BuildingBlocks.Shared;

namespace Product.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string? ImageUrl { get; private set; }
    public Guid? ParentCategoryId { get; private set; }
    public Category? ParentCategory { get; private set; }
    public ICollection<Product> Products { get; private set; } = [];

    private Category() { }

    public Category(string name, string description, string? imageUrl = null, Guid? parentCategoryId = null)
    {
        Name = name;
        Description = description;
        ImageUrl = imageUrl;
        ParentCategoryId = parentCategoryId;
    }

    public void UpdateDetails(string name, string description, string? imageUrl)
    {
        Name = name;
        Description = description;
        ImageUrl = imageUrl;
        MarkAsUpdated();
    }
}
