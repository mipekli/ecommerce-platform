using BuildingBlocks.Shared;

namespace Inventory.Domain.Entities;

public class StockItem : BaseEntity
{
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = null!;
    public string SKU { get; private set; } = null!;
    public int QuantityOnHand { get; private set; }
    public int ReservedQuantity { get; private set; }
    public int AvailableQuantity => QuantityOnHand - ReservedQuantity;
    public int LowStockThreshold { get; private set; } = 10;
    public string WarehouseLocation { get; private set; } = null!;

    private StockItem() { }

    public StockItem(Guid productId, string productName, string sku, int initialQuantity, string warehouseLocation = "Main")
    {
        ProductId = productId;
        ProductName = productName;
        SKU = sku;
        QuantityOnHand = initialQuantity;
        WarehouseLocation = warehouseLocation;
    }

    public void AddStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Eklenen miktar pozitif olmalıdır.");
        QuantityOnHand += quantity;
        MarkAsUpdated();
    }

    public void RemoveStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Çıkarılan miktar pozitif olmalıdır.");
        if (quantity > AvailableQuantity)
            throw new InvalidOperationException("Yetersiz stok.");
        QuantityOnHand -= quantity;
        MarkAsUpdated();
    }

    public void ReserveStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Rezerve miktar pozitif olmalıdır.");
        if (quantity > AvailableQuantity)
            throw new InvalidOperationException("Rezervasyon için yetersiz stok.");
        ReservedQuantity += quantity;
        MarkAsUpdated();
    }

    public void ReleaseReservedStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Serbest bırakılan miktar pozitif olmalıdır.");
        if (quantity > ReservedQuantity)
            throw new InvalidOperationException("Serbest bırakılacak rezerve stok miktarı aşıldı.");
        ReservedQuantity -= quantity;
        MarkAsUpdated();
    }

    public bool IsLowStock() => AvailableQuantity <= LowStockThreshold;

    public void UpdateLowStockThreshold(int threshold)
    {
        if (threshold < 0)
            throw new ArgumentException("Düşük stok eşiği negatif olamaz.");
        LowStockThreshold = threshold;
        MarkAsUpdated();
    }

    public void UpdateProductName(string productName)
    {
        ProductName = productName;
        MarkAsUpdated();
    }
}
