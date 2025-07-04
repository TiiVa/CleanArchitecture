using CleanArchitecture.Domain.Enums;

namespace CleanArchitecture.Application.DTOs.UserDtos;

public sealed record FileDataDto(string Name, byte[] Data, FileType Type);