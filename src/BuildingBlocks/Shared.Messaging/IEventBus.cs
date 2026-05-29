namespace BuildingBlocks.Shared.Messaging;

public interface IEventBus
{
    Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IntegrationEvent;
    Task SubscribeAsync<T, THandler>()
        where T : IntegrationEvent
        where THandler : IIntegrationEventHandler<T>;
    void RegisterHandler<T>(IIntegrationEventHandler<T> handler) where T : IntegrationEvent;
}
