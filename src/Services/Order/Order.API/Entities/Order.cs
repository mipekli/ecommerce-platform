using BuildingBlocks.Shared;
using Order.API.ValueObjects;
using Order.API.Events;

namespace Order.API.Entities;

public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    Shipped = 2,
    Delivered = 3,
    Cancelled = 4
}

public class Order : BaseEntity
{
    public Guid UserId { get; private set; }
    public string OrderNumber { get; private set; } = null!;
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;
    public Address ShippingAddress { get; private set; } = null!;
    public Address BillingAddress { get; private set; } = null!;
    public decimal SubTotal { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal ShippingCost { get; private set; }
    public decimal TotalAmount => SubTotal + TaxAmount + ShippingCost;
    public string? Notes { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    private readonly List<OrderItem> _items = [];

    private Order() { }

    public Order(Guid userId, Address shippingAddress, Address billingAddress)
    {
        UserId = userId;
        OrderNumber = GenerateOrderNumber();
        ShippingAddress = shippingAddress;
        BillingAddress = billingAddress;
    }

    public void AddItem(Guid productId, string productName, decimal unitPrice, int quantity)
    {
        var existingItem = _items.FirstOrDefault(i => i.ProductId == productId);
        if (existingItem is not null)
        {
            existingItem.UpdateQuantity(existingItem.Quantity + quantity);
        }
        else
        {
            _items.Add(new OrderItem(productId, productName, unitPrice, quantity));
        }
        RecalculateTotals();
        MarkAsUpdated();
    }

    public void RemoveItem(Guid productId)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item is not null)
        {
            _items.Remove(item);
            RecalculateTotals();
            MarkAsUpdated();
        }
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Yalnızca bekleyen siparişler onaylanabilir.");

        Status = OrderStatus.Confirmed;
        AddDomainEvent(new OrderCreatedDomainEvent(this));
        MarkAsUpdated();
    }

    public void Ship()
    {
        if (Status != OrderStatus.Confirmed)
            throw new InvalidOperationException("Yalnızca onaylanmış siparişler kargoya verilebilir.");

        Status = OrderStatus.Shipped;
        MarkAsUpdated();
    }

    public void Deliver()
    {
        if (Status != OrderStatus.Shipped)
            throw new InvalidOperationException("Yalnızca kargoya verilmiş siparişler teslim edilebilir.");

        Status = OrderStatus.Delivered;
        MarkAsUpdated();
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Delivered)
            throw new InvalidOperationException("Teslim edilmiş siparişler iptal edilemez.");

        Status = OrderStatus.Cancelled;
        MarkAsUpdated();
    }

    public void UpdateNotes(string notes)
    {
        Notes = notes;
        MarkAsUpdated();
    }

    private void RecalculateTotals()
    {
        SubTotal = _items.Sum(i => i.TotalPrice);
        TaxAmount = SubTotal * 0.18m;
    }

    private static string GenerateOrderNumber()
    {
        return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid():N}"[..20].ToUpper();
    }
}
