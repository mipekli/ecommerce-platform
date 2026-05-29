namespace BuildingBlocks.Shared.Messaging.SagaMessages;

public record ReserveStockCommand
{
    public Guid OrderId { get; init; }
    public List<StockItemRequest> Items { get; init; } = [];
}

public record StockItemRequest
{
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
}

public record StockReservedEvent
{
    public Guid OrderId { get; init; }
}

public record StockReservationFailedEvent
{
    public Guid OrderId { get; init; }
    public string Reason { get; init; } = string.Empty;
}

public record CancelOrderSagaCommand(Guid OrderId);
