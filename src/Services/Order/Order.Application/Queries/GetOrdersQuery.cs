using MediatR;
using Order.Application.DTOs;

namespace Order.Application.Queries;

public record GetOrdersQuery(Guid? UserId = null) : IRequest<IEnumerable<OrderDto>>;
