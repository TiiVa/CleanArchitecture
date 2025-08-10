using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Interfaces.RepositoryInterfaces;

public interface IStoryRepository : IRepository<Story>
{
    Task<Story> GetById(Guid id);
    Task<List<Story>> GetActive();
    Task<List<Story>> GetPublic();
}