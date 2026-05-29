using MediatR;
using BuildingBlocks.Shared.Messaging;
using Order.Domain.Interfaces;

namespace Order.Application.IntegrationEvents;

public class OrderCreatedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
{
    private readonly IMediator _mediator;
    private readonly IEventBus _eventBus;

    public OrderCreatedIntegrationEventHandler(IMediator mediator, IEventBus eventBus)
    {
        _mediator = mediator;
        _eventBus = eventBus;
    }

    public async Task HandleAsync(OrderCreatedIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        await _eventBus.PublishAsync(@event, cancellationToken);
    }
}
