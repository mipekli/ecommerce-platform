using MediatR;
using Product.API.DTOs;

namespace Product.API.Commands;

public record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    string ImageUrl,
    Guid CategoryId,
    string SKU) : IRequest<ProductDto>;
