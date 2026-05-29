using BuildingBlocks.Shared;

namespace Order.Domain.Events;

public record OrderCreatedDomainEvent(Entities.Order Order) : BaseEvent;
