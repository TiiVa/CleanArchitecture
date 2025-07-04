namespace CleanArchitecture.Application.DTOs.UserDtos;

public sealed record EventInvitationInfoForRegistrationDto(
    Guid EventId,
    string EventName,
    DateTime EventDate,
    string ContactEmail,
    string ContactInfo);