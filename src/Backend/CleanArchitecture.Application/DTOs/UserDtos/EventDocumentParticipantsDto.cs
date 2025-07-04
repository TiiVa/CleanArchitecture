namespace CleanArchitecture.Application.DTOs.UserDtos;

public sealed record EventDocumentsParticipantsDto(
    string EventName,
    string EventDescription,
    DateTime EventDate,
    List<FileNoDataDto> Files,
    List<ParticipantDto> Participants);