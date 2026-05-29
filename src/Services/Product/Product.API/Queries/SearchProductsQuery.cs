using MediatR;
using Product.API.DTOs;

namespace Product.API.Queries;

public record SearchProductsQuery(string SearchTerm) : IRequest<IEnumerable<ProductDto>>;
