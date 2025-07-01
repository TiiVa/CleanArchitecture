using CleanArchitecture.Application.Interfaces.RepositoryInterfaces;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Repositories;

public class DocumentRepository(ApplicationDbContext context) : IDocumentRepository
{
	public async Task<IEnumerable<Document>> GetAllAsync()
	{
		return await context.Documents.ToListAsync();
	}

	public async Task<Document> GetByIdAsync(Guid id)
	{
		var document = await context.Documents.FirstOrDefaultAsync(d => d.Id == id);
		if (document == null)
		{
			return new Document();
		}

		return document;
	}

	public async Task AddAsync(Document entity)
	{
		await context.Documents.AddAsync(entity);

		await context.SaveChangesAsync();
	}

	public async Task UpdateAsync(Document entity, Guid id)
	{
		var documentToUpdate = await context.Documents.FirstOrDefaultAsync(d => d.Id == id);
		var resource = await context.Resources.FirstOrDefaultAsync(r => r.Id == entity.ResourceId);
		
		// Behöver hämta applicationUser med hjälp av UserHandler?

		documentToUpdate.Name = entity.Name;
		documentToUpdate.Description = entity.Description;
		documentToUpdate.Type = entity.Type;
		documentToUpdate.File = entity.File;
		documentToUpdate.FileLocation = entity.FileLocation;
		documentToUpdate.ResourceId = entity.ResourceId;
		documentToUpdate.Resource = entity.Resource;
		documentToUpdate.ApplicationUserId = entity.ApplicationUserId;
		documentToUpdate.ApplicationUser = entity.ApplicationUser;

		await context.SaveChangesAsync();

	}

	public async Task DeleteAsync(Guid id)
	{
		var documentToDelete = await context.Documents.FirstOrDefaultAsync(e => e.Id == id);

		context.Documents.Remove(documentToDelete);

		await context.SaveChangesAsync();
	}
}