using MediatR;
using Product.API.Interfaces;
using Product.API.DTOs;
using ProductDomain = Product.API.Entities;

namespace Product.API.Commands;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public CreateProductCommandHandler(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category is null)
            throw new KeyNotFoundException("Kategori bulunamadı.");

        var existingProduct = await _productRepository.ExistsBySkuAsync(request.SKU, cancellationToken);
        if (existingProduct)
            throw new InvalidOperationException("Bu SKU ile kayıtlı bir ürün zaten var.");

        var product = new ProductDomain.Product(request.Name, request.Description, request.Price, request.ImageUrl, request.CategoryId, request.SKU);
        await _productRepository.AddAsync(product, cancellationToken);

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Currency = product.Currency,
            ImageUrl = product.ImageUrl,
            CategoryName = category.Name,
            CategoryId = product.CategoryId,
            SKU = product.SKU,
            IsPublished = product.IsPublished,
            CreatedAt = product.CreatedAt
        };
    }
}
