using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.DTOs.UserDtos;
using FluentResults;

namespace CleanArchitecture.Application.Interfaces.ServiceInterfaces;

public interface IEventService
{
    Task<Result> CreateEvent(EventDto? eventDto);
    Task<Result<IEnumerable<EventDto>>> GetAllEvents();
    Task<Result> UpdateEvent(EventDto eventDto, Guid id);
    Task<Result> DeleteEvent(Guid id);
    Task<Result<EventDocumentsParticipantsDto>> GetEventForUser(Guid id);
    Task<Result<List<EventInvitationInfoForRegistrationDto>>> GetEventsOpenForRegistration();
    Task<Result<List<EventSparseInfoDto>>> GetEventsVisibleToUser();
}