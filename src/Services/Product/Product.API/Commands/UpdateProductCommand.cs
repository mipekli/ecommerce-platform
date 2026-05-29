using MediatR;
using Product.API.DTOs;

namespace Product.API.Commands;

public record UpdateProductCommand(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string ImageUrl,
    Guid CategoryId) : IRequest<ProductDto>;
