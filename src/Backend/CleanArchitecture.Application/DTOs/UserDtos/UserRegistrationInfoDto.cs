using CleanArchitecture.Domain.Enums;

namespace CleanArchitecture.Application.DTOs.UserDtos;

public sealed record UserRegistrationInfoDto(
    string FullName,
    string Email,
    string Company,
    string Phone,
    Accommodation Accommodation,
    string AccommodationWith,
    string InvoiceReference,
    string Allergies,
    string UserInformation,
    string ContactInfo,
    string ContactEmail,
    string EventName,
    DateTime EventDate
);