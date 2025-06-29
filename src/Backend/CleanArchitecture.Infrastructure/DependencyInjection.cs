using CleanArchitecture.Application.Interfaces.RepositoryInterfaces;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Data;
using CleanArchitecture.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Infrastructure;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
	{

		var connectionString = config.GetConnectionString("DefaultConnection");
		services.AddDbContext<ApplicationDbContext>(options =>
		{
			options.UseSqlServer(connectionString);
		});

		services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
			.AddRoles<IdentityRole>()
			.AddEntityFrameworkStores<ApplicationDbContext>()
			.AddSignInManager()
			.AddDefaultTokenProviders();

		services.AddScoped<IDocumentRepository, DocumentRepository>();

		return services;
	}
}