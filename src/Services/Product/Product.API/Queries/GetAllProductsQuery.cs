using MediatR;
using Product.API.DTOs;

namespace Product.API.Queries;

public record GetAllProductsQuery : IRequest<IEnumerable<ProductDto>>;
