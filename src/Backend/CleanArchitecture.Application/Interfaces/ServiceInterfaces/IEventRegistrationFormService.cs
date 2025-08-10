using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.DTOs.UserDtos;
using CleanArchitecture.Domain.Entities;
using FluentResults;

namespace CleanArchitecture.Application.Interfaces.ServiceInterfaces;

public interface IEventRegistrationFormService
{
    Task<Result<IEnumerable<EventRegistrationFormDto>>> GetAllAsync();
    Task<Result> AddAsync(EventRegistrationSparseDto? userInput);
   Task<Result> UpdateFormAsync(EventRegistrationFormDto form, Guid id);
    Task<Result> DeleteFormAsync(Guid id);
    Task<Result<List<UserRegistrationInfoDto>>> GetRegistrationInfoByUserIdAsync(Guid id);

    Task<Result> ConfirmParticipationInEvent(string userId, string eventId);
    Task<Result> SendWelcomeMessageWithAccountConfirmation(EventRegistrationForm user, string password);
    Task<Result> CreateDefaultAccountAsync(string email, string password);

    Task<Result<string>> CreateWelcomeMessageWithAccountConfirmationLinkAsync(string email, string password,
        Guid eventId);
}