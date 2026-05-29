using MediatR;
using Order.Domain.Interfaces;

namespace Order.Application.Commands;

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Unit>
{
    private readonly IOrderRepository _orderRepository;

    public CancelOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Unit> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
            throw new KeyNotFoundException("Sipariş bulunamadı.");

        order.Cancel();
        await _orderRepository.UpdateAsync(order, cancellationToken);
        return Unit.Value;
    }
}
