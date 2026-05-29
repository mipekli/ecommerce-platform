using MediatR;
using Order.API.DTOs;

namespace Order.API.Queries;

public record GetOrderByIdQuery(Guid Id) : IRequest<OrderDto>;
