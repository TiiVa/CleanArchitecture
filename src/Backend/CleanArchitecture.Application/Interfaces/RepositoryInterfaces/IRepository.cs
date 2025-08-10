namespace CleanArchitecture.Application.Interfaces.RepositoryInterfaces;

public interface IRepository<TEntity> where TEntity : class
{

    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<bool> AddAsync(TEntity entity);
    Task<bool> UpdateAsync(TEntity entity, Guid id);
    Task<bool> RemoveAsync(Guid id);
}