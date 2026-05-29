using BuildingBlocks.Shared.Messaging;

namespace Inventory.API.IntegrationEvents.OrderCreated;

// Re-use the same event type name for message routing
public record OrderCreatedEvent : IntegrationEvent
{
    public Guid OrderId { get; init; }
    public List<OrderItemEvent> Items { get; init; } = [];
}

public record OrderItemEvent
{
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
}
