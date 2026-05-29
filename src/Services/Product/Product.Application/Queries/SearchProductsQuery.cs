using MediatR;
using Product.Application.DTOs;

namespace Product.Application.Queries;

public record SearchProductsQuery(string SearchTerm) : IRequest<IEnumerable<ProductDto>>;
