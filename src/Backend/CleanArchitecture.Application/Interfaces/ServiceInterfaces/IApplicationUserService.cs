using CleanArchitecture.Application.DTOs;
using FluentResults;

namespace CleanArchitecture.Application.Interfaces.ServiceInterfaces;

public interface IApplicationUserService
{
   Task<Result<ApplicationUserDto>> GetUserByEmailAsync(string email);
   
}