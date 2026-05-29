using MediatR;
using Order.API.DTOs;

namespace Order.API.Commands;

public record ConfirmOrderCommand(Guid OrderId) : IRequest<OrderDto>;
