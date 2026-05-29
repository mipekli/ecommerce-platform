using Microsoft.AspNetCore.Mvc;
using Product.API.Interfaces;
using Product.API.DTOs;
using BuildingBlocks.Shared.Caching;

namespace Product.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICacheService _cacheService;

    public CategoriesController(ICategoryRepository categoryRepository, ICacheService cacheService)
    {
        _categoryRepository = categoryRepository;
        _cacheService = cacheService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll(CancellationToken cancellationToken)
    {
        const string cacheKey = "categories:all";
        var cached = await _cacheService.GetAsync<IEnumerable<CategoryDto>>(cacheKey, cancellationToken);
        if (cached is not null)
            return Ok(cached);

        var categories = await _categoryRepository.GetAllAsync(cancellationToken);
        var dtos = categories.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            ImageUrl = c.ImageUrl,
            ProductCount = c.Products.Count
        });

        await _cacheService.SetAsync(cacheKey, dtos, TimeSpan.FromMinutes(10), cancellationToken);
        return Ok(dtos);
    }
}
