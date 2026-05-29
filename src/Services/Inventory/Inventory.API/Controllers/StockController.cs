using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Inventory.Domain.Interfaces;
using Inventory.Domain.Entities;

namespace Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class StockController : ControllerBase
{
    private readonly IStockItemRepository _stockItemRepository;

    public StockController(IStockItemRepository stockItemRepository)
    {
        _stockItemRepository = stockItemRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StockItem>>> GetAll(CancellationToken cancellationToken)
    {
        var items = await _stockItemRepository.GetAllAsync(cancellationToken);
        return Ok(items);
    }

    [HttpGet("low-stock")]
    public async Task<ActionResult<IEnumerable<StockItem>>> GetLowStock(CancellationToken cancellationToken)
    {
        var items = await _stockItemRepository.GetLowStockItemsAsync(cancellationToken);
        return Ok(items);
    }

    [HttpGet("product/{productId:guid}")]
    public async Task<ActionResult<StockItem>> GetByProductId(Guid productId, CancellationToken cancellationToken)
    {
        var item = await _stockItemRepository.GetByProductIdAsync(productId, cancellationToken);
        if (item is null)
            return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<StockItem>> Create(
        [FromBody] CreateStockRequest request, CancellationToken cancellationToken)
    {
        var existing = await _stockItemRepository.GetByProductIdAsync(request.ProductId, cancellationToken);
        if (existing is not null)
            return BadRequest("Bu ürün için stok kaydı zaten mevcut.");

        var stockItem = new StockItem(request.ProductId, request.ProductName, request.SKU, request.InitialQuantity, request.WarehouseLocation);
        await _stockItemRepository.AddAsync(stockItem, cancellationToken);
        return CreatedAtAction(nameof(GetByProductId), new { productId = stockItem.ProductId }, stockItem);
    }

    [HttpPut("{id:guid}/add-stock")]
    public async Task<ActionResult> AddStock(Guid id, [FromBody] int quantity, CancellationToken cancellationToken)
    {
        var item = await _stockItemRepository.GetByIdAsync(id, cancellationToken);
        if (item is null) return NotFound();
        item.AddStock(quantity);
        await _stockItemRepository.UpdateAsync(item, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:guid}/remove-stock")]
    public async Task<ActionResult> RemoveStock(Guid id, [FromBody] int quantity, CancellationToken cancellationToken)
    {
        var item = await _stockItemRepository.GetByIdAsync(id, cancellationToken);
        if (item is null) return NotFound();
        item.RemoveStock(quantity);
        await _stockItemRepository.UpdateAsync(item, cancellationToken);
        return NoContent();
    }
}

public record CreateStockRequest
{
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string SKU { get; init; } = string.Empty;
    public int InitialQuantity { get; init; }
    public string WarehouseLocation { get; init; } = "Main";
}
