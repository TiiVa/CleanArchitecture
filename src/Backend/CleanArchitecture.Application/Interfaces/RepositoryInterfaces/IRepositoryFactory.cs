namespace CleanArchitecture.Application.Interfaces.RepositoryInterfaces;

public interface IRepositoryFactory
{
    TEntity? CreateRepository<TEntity>() where TEntity : class;
}