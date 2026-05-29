using BuildingBlocks.Shared.Messaging;

namespace Order.Application.IntegrationEvents;

public record OrderCreatedIntegrationEvent : IntegrationEvent
{
    public Guid OrderId { get; init; }
    public Guid UserId { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public List<OrderItemEvent> Items { get; init; } = [];
    public DateTime CreatedAt { get; init; }
}

public record OrderItemEvent
{
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public decimal UnitPrice { get; init; }
    public int Quantity { get; init; }
}
