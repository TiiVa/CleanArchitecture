using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Interfaces.RepositoryInterfaces;

public interface IInvitationRepository : IRepository<Invitation>
{
    Task<Invitation> GetByIdAsync(Guid id);
    Task<List<Invitation>> GetInvitationsForEventsOpenForRegistrationAsync();
}