using CleanArchitecture.Application.Interfaces.RepositoryInterfaces;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Repositories;

public class ApplicationUserRepository : IApplicationUserRepository
{
    private readonly ApplicationDbContext _context;
    
    public ApplicationUserRepository(ApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<ApplicationUser> GetAllInfoByEmailAsync(string email)
    {
        var userByEmail = await _context.Users
            .Include(u => u.Documents)
            .Include(u => u.Events)
            .Include(u => u.Resources)
            .Include(u => u.Stories)
            .AsSplitQuery()
            .FirstOrDefaultAsync(u => u.Email == email);

        if (userByEmail is null) return new ApplicationUser();

        return userByEmail;
    }
    
}