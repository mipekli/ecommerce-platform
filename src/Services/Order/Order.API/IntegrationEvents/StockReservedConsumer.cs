using MassTransit;
using BuildingBlocks.Shared.Messaging.SagaMessages;

namespace Order.API.Sagas;

public class StockReservedConsumer : IConsumer<StockReservedEvent>
{
    private readonly ILogger<StockReservedConsumer> _logger;

    public StockReservedConsumer(ILogger<StockReservedConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<StockReservedEvent> context)
    {
        _logger.LogInformation("Stock reserved for order {OrderId}", context.Message.OrderId);
        return Task.CompletedTask;
    }
}

public class StockReservationFailedConsumer : IConsumer<StockReservationFailedEvent>
{
    private readonly ILogger<StockReservationFailedConsumer> _logger;

    public StockReservationFailedConsumer(ILogger<StockReservationFailedConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<StockReservationFailedEvent> context)
    {
        _logger.LogWarning("Stock reservation failed for order {OrderId}: {Reason}",
            context.Message.OrderId, context.Message.Reason);
        return Task.CompletedTask;
    }
}
