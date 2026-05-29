using BuildingBlocks.Shared.Messaging;
using Inventory.API.IntegrationEvents.OrderCreated;

namespace Inventory.API.BackgroundServices;

public class OrderCreatedConsumerService : BackgroundService
{
    private readonly IEventBus _eventBus;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OrderCreatedConsumerService> _logger;

    public OrderCreatedConsumerService(
        IEventBus eventBus,
        IServiceProvider serviceProvider,
        ILogger<OrderCreatedConsumerService> logger)
    {
        _eventBus = eventBus;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OrderCreated consumer service starting...");

        var handler = new OrderCreatedEventHandler(
            _serviceProvider.GetRequiredService<Inventory.API.Interfaces.IStockItemRepository>());

        _eventBus.RegisterHandler(handler);
        await _eventBus.SubscribeAsync<OrderCreatedEvent, OrderCreatedEventHandler>();

        _logger.LogInformation("OrderCreated consumer service started successfully.");

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
