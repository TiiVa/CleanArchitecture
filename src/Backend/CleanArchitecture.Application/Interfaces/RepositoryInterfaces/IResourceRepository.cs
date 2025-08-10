using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Interfaces.RepositoryInterfaces;

public interface IResourceRepository : IRepository<Resource>
{
    Task<Resource> GetByIdAsync(Guid id);
}