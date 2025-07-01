using CleanArchitecture.Domain.Entities.Interfaces;

namespace CleanArchitecture.Application.Interfaces.RepositoryInterfaces;

public interface IRepository<TEntity, TId> where TEntity : IEntity<TId>
{
	Task<IEnumerable<TEntity>> GetAllAsync();
	Task<TEntity> GetByIdAsync(Guid id);
	Task AddAsync(TEntity entity);
	Task UpdateAsync(TEntity entity, Guid id);
	Task DeleteAsync(Guid id);
}