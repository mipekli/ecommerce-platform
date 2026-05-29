using MediatR;
using Order.Application.DTOs;

namespace Order.Application.Commands;

public record ConfirmOrderCommand(Guid OrderId) : IRequest<OrderDto>;
