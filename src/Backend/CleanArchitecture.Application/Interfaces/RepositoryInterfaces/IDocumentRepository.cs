using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Interfaces.RepositoryInterfaces;

public interface IDocumentRepository : IRepository<Document, Guid>
{
	
}