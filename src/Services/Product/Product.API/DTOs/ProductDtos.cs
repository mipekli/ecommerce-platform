namespace Product.API.DTOs;

public record ProductDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string Currency { get; init; } = "TRY";
    public string ImageUrl { get; init; } = string.Empty;
    public string CategoryName { get; init; } = string.Empty;
    public Guid CategoryId { get; init; }
    public string SKU { get; init; } = string.Empty;
    public bool IsPublished { get; init; }
    public DateTime CreatedAt { get; init; }
}

public record CreateProductRequest
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string ImageUrl { get; init; } = string.Empty;
    public Guid CategoryId { get; init; }
    public string SKU { get; init; } = string.Empty;
}

public record UpdateProductRequest
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string ImageUrl { get; init; } = string.Empty;
    public Guid CategoryId { get; init; }
}

public record CategoryDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string? ImageUrl { get; init; }
    public int ProductCount { get; init; }
}
