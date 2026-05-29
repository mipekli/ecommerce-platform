using Microsoft.EntityFrameworkCore;
using Inventory.API.Entities;
using Inventory.API.Interfaces;
using Inventory.API.Data;

namespace Inventory.API.Repositories;

public class StockItemRepository : IStockItemRepository
{
    private readonly InventoryDbContext _context;

    public StockItemRepository(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<StockItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.StockItems.FindAsync([id], cancellationToken);
    }

    public async Task<StockItem?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _context.StockItems.FirstOrDefaultAsync(s => s.ProductId == productId, cancellationToken);
    }

    public async Task<StockItem?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        return await _context.StockItems.FirstOrDefaultAsync(s => s.SKU == sku, cancellationToken);
    }

    public async Task<IEnumerable<StockItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.StockItems.OrderBy(s => s.ProductName).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<StockItem>> GetLowStockItemsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.StockItems
            .Where(s => (s.QuantityOnHand - s.ReservedQuantity) <= s.LowStockThreshold)
            .OrderBy(s => s.ProductName)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(StockItem stockItem, CancellationToken cancellationToken = default)
    {
        await _context.StockItems.AddAsync(stockItem, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(StockItem stockItem, CancellationToken cancellationToken = default)
    {
        _context.StockItems.Update(stockItem);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(StockItem stockItem, CancellationToken cancellationToken = default)
    {
        stockItem.MarkAsDeleted();
        _context.StockItems.Update(stockItem);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
