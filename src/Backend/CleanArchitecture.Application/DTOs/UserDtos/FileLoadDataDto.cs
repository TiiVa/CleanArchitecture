using CleanArchitecture.Domain.Enums;

namespace CleanArchitecture.Application.DTOs.UserDtos;

public sealed record FileNoDataDto(Guid Id, string Name, FileType Type);