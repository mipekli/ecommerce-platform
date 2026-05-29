using MediatR;
using Product.Application.DTOs;

namespace Product.Application.Commands;

public record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    string ImageUrl,
    Guid CategoryId,
    string SKU) : IRequest<ProductDto>;
