using MassTransit;
using Order.API.Interfaces;
using BuildingBlocks.Shared.Messaging.SagaMessages;

namespace Order.API.Consumers;

public class CancelOrderConsumer : IConsumer<CancelOrderSagaCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<CancelOrderConsumer> _logger;

    public CancelOrderConsumer(
        IOrderRepository orderRepository,
        ILogger<CancelOrderConsumer> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CancelOrderSagaCommand> context)
    {
        _logger.LogInformation("Compensating: Cancelling order {OrderId}", context.Message.OrderId);

        var order = await _orderRepository.GetByIdAsync(context.Message.OrderId, context.CancellationToken);
        if (order is null)
        {
            _logger.LogWarning("Order {OrderId} not found for cancellation", context.Message.OrderId);
            return;
        }

        order.Cancel();
        await _orderRepository.UpdateAsync(order, context.CancellationToken);

        _logger.LogInformation("Order {OrderNumber} cancelled as compensating transaction", order.OrderNumber);
    }
}
