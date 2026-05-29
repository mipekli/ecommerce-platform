namespace Order.Domain.Interfaces;

using Entities;

public interface IOrderRepository
{
    Task<Entities.Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Entities.Order>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Entities.Order>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Entities.Order order, CancellationToken cancellationToken = default);
    Task UpdateAsync(Entities.Order order, CancellationToken cancellationToken = default);
    Task DeleteAsync(Entities.Order order, CancellationToken cancellationToken = default);
}
