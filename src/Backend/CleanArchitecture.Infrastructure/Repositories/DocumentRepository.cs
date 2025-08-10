using CleanArchitecture.Application.Interfaces.RepositoryInterfaces;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Repositories;

public class DocumentRepository : IDocumentRepository
{
    private readonly ApplicationDbContext _context;
    

    public DocumentRepository(ApplicationDbContext context)
    {
        _context = context;
       
    }
    public async Task<IEnumerable<Document>> GetAllAsync()
    {
        var documents = await _context.Documents
            .Include(d => d.Resource)
            .Include(x => x.ApplicationUser)
            .ToListAsync();

        return documents;
    }
    public async Task<Document> GetByIdAsync(Guid id)
    {
        var document = await _context.Documents
            .Include(d => d.Resource)
            .Include(x => x.ApplicationUser)
            .Where(d => d.Id == id).FirstOrDefaultAsync();

        if (document is null) return new Document();

        return document;
    }

    public async Task<bool> AddAsync(Document entity)
    {
        var newDocument = await _context.Documents.AddAsync(entity);

        if (newDocument is null) return false;

        return true;
    }

    public async Task<bool> UpdateAsync(Document entity, Guid id)
    {
        var documentToUpdate = await _context.Documents.FirstOrDefaultAsync(d => d.Id == id);

        if (documentToUpdate is null) return false;

        documentToUpdate.Name = entity.Name;
        documentToUpdate.Description = entity.Description;
        documentToUpdate.Type = entity.Type;

        if (entity.File is not null)
        {
            documentToUpdate.File = entity.File;
        }

        return true;
    }

    public async Task<bool> RemoveAsync(Guid id)
    {
        var documentToDelete = await _context.Documents.FirstOrDefaultAsync(d => d.Id == id);

        if (documentToDelete is null) return false;

        _context.Documents.Remove(documentToDelete);

        return true;
    }

    public async Task<List<Document>> GetDocumentsByResourceId(Guid resourceId)
    {
        var documents = await _context.Documents
            .Include(x => x.ApplicationUser)
            .Include(x => x.Resource)
            .Where(x => x.Resource.Id.Equals(resourceId))
            .ToListAsync();

        return documents;
    }

    public async Task<List<Document>> GetImagesForUserByEventId(Guid eventId)
    {
        var documents = await _context
            .Documents
            .Include(document => document.Resource)
            .ThenInclude(resource => resource.EventResources)
            .ThenInclude(eventResource => eventResource.Event)
            .Where(document => document.Resource.EventResources.Any(eventResource => eventResource.EventId.Equals(eventId) && eventResource.Event.IsVisibleToUser))
            .ToListAsync();


        return documents;
    }
}