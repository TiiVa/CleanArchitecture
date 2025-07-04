using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.DTOs.Converters;

public static class ApplicationUserConvertTo
{
    public static ApplicationUserDto ConvertToDto(this ApplicationUser u)
    {
        var d = new ApplicationUserDto
        {
            Id = Guid.Parse(u.Id),
            UserName = u.UserName,
            Email = u.Email,
            Password = u.PasswordHash
        };

        return d;
    }

    public static ApplicationUser ConvertToModel(this ApplicationUserDto d)
    {
        var m = new ApplicationUser
        {
            Id = d.Id.ToString(),
            UserName = d.UserName,
            Email = d.Email,
            PasswordHash = d.Password
        };

        return m;
    }
}