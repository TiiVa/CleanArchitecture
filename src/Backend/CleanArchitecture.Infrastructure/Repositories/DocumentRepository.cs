using System.Reflection.Metadata.Ecma335;
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

	public async Task<Document> GetByIdAsync(int id)
	{
		throw new NotImplementedException();
	}

	public async Task AddAsync(Document entity)
	{
		throw new NotImplementedException();
	}

	public async Task UpdateAsync(Document entity, int id)
	{
		throw new NotImplementedException();
	}

	public async Task DeleteAsync(int id)
	{
		throw new NotImplementedException();
	}
}