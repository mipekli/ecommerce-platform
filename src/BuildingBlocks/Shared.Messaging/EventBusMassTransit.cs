using MassTransit;

namespace BuildingBlocks.Shared.Messaging;

public class EventBusMassTransit : IEventBus
{
    private readonly IPublishEndpoint _publishEndpoint;

    public EventBusMassTransit(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class
    {
        await _publishEndpoint.Publish(@event, cancellationToken);
    }
}
