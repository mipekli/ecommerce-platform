using MediatR;

namespace Order.API.Commands;

public record CancelOrderCommand(Guid OrderId) : IRequest<Unit>;
