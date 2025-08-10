using CleanArchitecture.Application.DTOs;
using FluentResults;

namespace CleanArchitecture.Application.Interfaces.ServiceInterfaces;

public interface IInvitationService
{
    Task<Result> CreateInvitation(InvitationDto invitationDto);
    Task<Result<InvitationDto>> GetInvitationById(Guid id);
    Task<Result<IEnumerable<InvitationDto>>> GetAllInvitations();
    Task<Result<List<InvitationDto>>> GetInvitationsForEventsOpenForRegistration();
    Task<Result> UpdateInvitation(InvitationDto invitationDto, Guid id);
    Task<Result> DeleteInvitation(Guid id);
}