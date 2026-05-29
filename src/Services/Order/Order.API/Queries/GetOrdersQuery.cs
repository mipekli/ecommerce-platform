using MediatR;
using Order.API.DTOs;

namespace Order.API.Queries;

public record GetOrdersQuery(Guid? UserId = null) : IRequest<IEnumerable<OrderDto>>;
