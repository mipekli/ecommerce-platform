using Inventory.API.Entities;

namespace Inventory.API.Interfaces;

public interface IStockItemRepository
{
    Task<StockItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<StockItem?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<StockItem?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockItem>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<StockItem>> GetLowStockItemsAsync(CancellationToken cancellationToken = default);
    Task AddAsync(StockItem stockItem, CancellationToken cancellationToken = default);
    Task UpdateAsync(StockItem stockItem, CancellationToken cancellationToken = default);
    Task DeleteAsync(StockItem stockItem, CancellationToken cancellationToken = default);
}
