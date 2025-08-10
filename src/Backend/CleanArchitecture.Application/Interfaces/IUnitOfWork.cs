namespace CleanArchitecture.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
	TEntity CreateRepository<TEntity>() where TEntity : class;

	Task CompleteAsync();

}