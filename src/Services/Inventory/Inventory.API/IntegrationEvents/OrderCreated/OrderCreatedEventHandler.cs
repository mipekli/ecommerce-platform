using BuildingBlocks.Shared.Messaging;
using Inventory.API.Interfaces;

namespace Inventory.API.IntegrationEvents.OrderCreated;

public class OrderCreatedEventHandler : IIntegrationEventHandler<OrderCreatedEvent>
{
    private readonly IStockItemRepository _stockItemRepository;

    public OrderCreatedEventHandler(IStockItemRepository stockItemRepository)
    {
        _stockItemRepository = stockItemRepository;
    }

    public async Task HandleAsync(OrderCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        foreach (var item in @event.Items)
        {
            var stockItem = await _stockItemRepository.GetByProductIdAsync(item.ProductId, cancellationToken);
            if (stockItem is null)
                throw new KeyNotFoundException($"Product {item.ProductId} için stok kaydı bulunamadı.");

            stockItem.ReserveStock(item.Quantity);
            await _stockItemRepository.UpdateAsync(stockItem, cancellationToken);
        }
    }
}
