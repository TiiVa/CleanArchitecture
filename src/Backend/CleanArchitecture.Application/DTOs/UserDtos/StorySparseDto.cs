namespace CleanArchitecture.Application.DTOs.UserDtos;

public sealed record StorySparseDto(string Header, string Body, DateTime Date, Guid EventId);