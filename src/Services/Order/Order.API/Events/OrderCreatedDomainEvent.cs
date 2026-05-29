using BuildingBlocks.Shared;

namespace Order.API.Events;

public record OrderCreatedDomainEvent(Entities.Order Order) : BaseEvent;
