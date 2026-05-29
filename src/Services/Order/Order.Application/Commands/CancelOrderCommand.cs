using MediatR;

namespace Order.Application.Commands;

public record CancelOrderCommand(Guid OrderId) : IRequest<Unit>;
