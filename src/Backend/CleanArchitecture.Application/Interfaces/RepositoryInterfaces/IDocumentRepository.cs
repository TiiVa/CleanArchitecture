using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Interfaces.RepositoryInterfaces;

public interface IDocumentRepository : IRepository<Document>
{
    Task<Document> GetByIdAsync(Guid id);
    Task<List<Document>> GetDocumentsByResourceId(Guid resourceId);
    Task<List<Document>> GetImagesForUserByEventId(Guid eventId);

}