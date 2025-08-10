using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Interfaces.RepositoryInterfaces;

public interface IApplicationUserRepository
{
    Task<ApplicationUser> GetAllInfoByEmailAsync(string email);
}