using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Product.API.Commands;
using Product.API.DTOs;
using Product.API.Queries;
using BuildingBlocks.Shared.Caching;

namespace Product.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICacheService _cacheService;

    public ProductsController(IMediator mediator, ICacheService cacheService)
    {
        _mediator = mediator;
        _cacheService = cacheService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll(CancellationToken cancellationToken)
    {
        const string cacheKey = "products:all";
        var cached = await _cacheService.GetAsync<IEnumerable<ProductDto>>(cacheKey, cancellationToken);
        if (cached is not null)
            return Ok(cached);

        var products = await _mediator.Send(new GetAllProductsQuery(), cancellationToken);
        await _cacheService.SetAsync(cacheKey, products, TimeSpan.FromMinutes(5), cancellationToken);
        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var cacheKey = $"products:{id}";
        var cached = await _cacheService.GetAsync<ProductDto>(cacheKey, cancellationToken);
        if (cached is not null)
            return Ok(cached);

        var product = await _mediator.Send(new GetProductByIdQuery(id), cancellationToken);
        await _cacheService.SetAsync(cacheKey, product, TimeSpan.FromMinutes(5), cancellationToken);
        return Ok(product);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> Search(
        [FromQuery] string q, CancellationToken cancellationToken)
    {
        var products = await _mediator.Send(new SearchProductsQuery(q), cancellationToken);
        return Ok(products);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(
        CreateProductRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateProductCommand(
            request.Name, request.Description, request.Price,
            request.ImageUrl, request.CategoryId, request.SKU);
        var product = await _mediator.Send(command, cancellationToken);
        await _cacheService.RemoveAsync("products:all", cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProductDto>> Update(
        Guid id, UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateProductCommand(
            id, request.Name, request.Description, request.Price,
            request.ImageUrl, request.CategoryId);
        var product = await _mediator.Send(command, cancellationToken);
        await _cacheService.RemoveAsync($"products:{id}", cancellationToken);
        await _cacheService.RemoveAsync("products:all", cancellationToken);
        return Ok(product);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteProductCommand(id), cancellationToken);
        await _cacheService.RemoveAsync($"products:{id}", cancellationToken);
        await _cacheService.RemoveAsync("products:all", cancellationToken);
        return NoContent();
    }
}
