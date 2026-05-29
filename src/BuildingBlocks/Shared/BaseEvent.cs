using MediatR;

namespace BuildingBlocks.Shared;

public abstract record BaseEvent : INotification
{
    public Guid EventId { get; protected set; } = Guid.NewGuid();
    public DateTime OccurredOn { get; protected set; } = DateTime.UtcNow;
    public string EventType => GetType().Name;
}
