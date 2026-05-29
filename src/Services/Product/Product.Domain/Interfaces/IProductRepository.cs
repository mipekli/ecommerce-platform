namespace Product.Domain.Interfaces;

using Entities;

public interface IProductRepository
{
    Task<Entities.Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Entities.Product>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Entities.Product>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Entities.Product>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<bool> ExistsBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task AddAsync(Entities.Product product, CancellationToken cancellationToken = default);
    Task UpdateAsync(Entities.Product product, CancellationToken cancellationToken = default);
    Task DeleteAsync(Entities.Product product, CancellationToken cancellationToken = default);
}
