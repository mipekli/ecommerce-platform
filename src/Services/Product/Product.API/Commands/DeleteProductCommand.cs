using MediatR;

namespace Product.API.Commands;

public record DeleteProductCommand(Guid Id) : IRequest<Unit>;
