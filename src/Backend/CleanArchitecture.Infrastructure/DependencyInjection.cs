using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Application.Interfaces.RepositoryInterfaces;
using CleanArchitecture.Application.Interfaces.ServiceInterfaces;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Data;
using CleanArchitecture.Infrastructure.ExternalServices;
using CleanArchitecture.Infrastructure.ExternalServices.Excel;
using CleanArchitecture.Infrastructure.ExternalServices.SeriLog;
using CleanArchitecture.Infrastructure.FactoryForExternalServices;
using CleanArchitecture.Infrastructure.FactoryForRepositories;
using CleanArchitecture.Infrastructure.Repositories;
using CleanArchitecture.Infrastructure.UOW;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
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






		services.AddScoped<IRepositoryFactory, RepositoryFactory>();
		services.AddScoped<IUnitOfWork, UnitOfWork>();

		// External Service Factory
		services.AddScoped<IExternalServiceFactory, ExternalServiceFactory>();


		// External services

		services.AddSingleton<ISerilogLogger, SerilogLogger>();
		services.AddScoped<IWorkBook, WorkBook>();
		services.AddScoped<IEmailService, EmailService>();
		services.AddScoped<IEmailSender, EmailService>();
		services.AddScoped<IPasswordGeneratorService, PasswordGeneratorService>();

		return services;
	}
}