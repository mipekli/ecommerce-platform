using MediatR;
using Product.API.DTOs;

namespace Product.API.Queries;

public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto>;
