using CleanArchitecture.Application.Interfaces.RepositoryInterfaces;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Repositories;

public class ResourceRepository : IResourceRepository
{
    private readonly ApplicationDbContext _context;

    public ResourceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Resource>> GetAllAsync()
    {
        var resources = await _context.Resources
            .Include(r => r.Documents).ThenInclude(d => d.ApplicationUser)
            .Include(r => r.ApplicationUser)
            .Include(r => r.EventResources)
            .ThenInclude(er => er.Event)
            .Select(r => new
            {
                Resource = r,
                DocumentCount = r.Documents.Count()
            })
            .ToListAsync();

        return resources.Select(x => x.Resource);
    }
    public async Task<Resource> GetByIdAsync(Guid id)
    {
        var resource = await _context.Resources
            .Include(r => r.Documents)
            .Include(r => r.ApplicationUser)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (resource is null) return null;
        
        return resource;
    }

    public async Task<bool> AddAsync(Resource entity)
    {
        var newResource = await _context.Resources.AddAsync(entity);

        if (newResource is null) return false;

        return true;
    }

    public async Task<bool> UpdateAsync(Resource entity, Guid id)
    {
        var resourceToUpdate = await _context.Resources.FirstOrDefaultAsync(r => r.Id == id);

        if (resourceToUpdate is null) return false;
       
        resourceToUpdate.ResourceName = entity.ResourceName;
        resourceToUpdate.ResourceType = entity.ResourceType;
        resourceToUpdate.Context = entity.Context;
        resourceToUpdate.ResourceDescription = entity.ResourceDescription;

        return true;
    }

    public async Task<bool> RemoveAsync(Guid id)
    {
        var resourceToDelete = await _context.Resources.FirstOrDefaultAsync(r => r.Id == id);
        
        if (resourceToDelete is null) return false;

        _context.Remove(resourceToDelete);

        return true;
    }

    
}