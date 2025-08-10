using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Interfaces.RepositoryInterfaces;

public interface IEventRegistrationFormRepository : IRepository<EventRegistrationForm>
{
    Task<List<EventRegistrationForm>> GetRegistrationInfoByUserIdAsync(Guid id);
    Task<bool> ConfirmParticipationInEvent(string userId, string eventId);
    Task<bool> UserIsAlreadyRegisteredToEvent(EventRegistrationForm user);
}