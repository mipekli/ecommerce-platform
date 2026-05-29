using MassTransit;
using Inventory.API.Interfaces;
using BuildingBlocks.Shared.Messaging.SagaMessages;

namespace Inventory.API.Consumers;

public class ReserveStockConsumer : IConsumer<ReserveStockCommand>
{
    private readonly IStockItemRepository _stockItemRepository;
    private readonly ILogger<ReserveStockConsumer> _logger;

    public ReserveStockConsumer(
        IStockItemRepository stockItemRepository,
        ILogger<ReserveStockConsumer> logger)
    {
        _stockItemRepository = stockItemRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ReserveStockCommand> context)
    {
        _logger.LogInformation("Reserving stock for order {OrderId}", context.Message.OrderId);

        try
        {
            foreach (var item in context.Message.Items)
            {
                var stockItem = await _stockItemRepository.GetByProductIdAsync(item.ProductId, context.CancellationToken);
                if (stockItem is null)
                {
                    await context.Publish(new StockReservationFailedEvent
                    {
                        OrderId = context.Message.OrderId,
                        Reason = $"Product {item.ProductId} iГ§in stok kaydД± bulunamadД±."
                    });
                    return;
                }

                if (stockItem.AvailableQuantity < item.Quantity)
                {
                    await context.Publish(new StockReservationFailedEvent
                    {
                        OrderId = context.Message.OrderId,
                        Reason = $"Product {item.ProductId} iГ§in yetersiz stok. Mevcut: {stockItem.AvailableQuantity}, Talep: {item.Quantity}"
                    });
                    return;
                }

                stockItem.ReserveStock(item.Quantity);
                await _stockItemRepository.UpdateAsync(stockItem, context.CancellationToken);
            }

            await context.Publish(new StockReservedEvent
            {
                OrderId = context.Message.OrderId
            });

            _logger.LogInformation("Stock successfully reserved for order {OrderId}", context.Message.OrderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reserving stock for order {OrderId}", context.Message.OrderId);
            await context.Publish(new StockReservationFailedEvent
            {
                OrderId = context.Message.OrderId,
                Reason = ex.Message
            });
        }
    }
}
