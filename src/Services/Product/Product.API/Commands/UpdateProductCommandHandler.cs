using MediatR;
using Product.API.Interfaces;
using Product.API.DTOs;

namespace Product.API.Commands;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public UpdateProductCommandHandler(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null)
            throw new KeyNotFoundException("Ürün bulunamadı.");

        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category is null)
            throw new KeyNotFoundException("Kategori bulunamadı.");

        product.UpdateDetails(request.Name, request.Description, request.Price, request.ImageUrl, request.CategoryId);
        await _productRepository.UpdateAsync(product, cancellationToken);

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
