using MediatR;
using Order.Application.DTOs;

namespace Order.Application.Queries;

public record GetOrderByIdQuery(Guid Id) : IRequest<OrderDto>;
