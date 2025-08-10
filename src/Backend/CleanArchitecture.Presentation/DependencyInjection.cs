using CleanArchitecture.Application;
using CleanArchitecture.Infrastructure;
using CleanArchitecture.Infrastructure.ExternalServices.Email;
using CleanArchitecture.Presentation.FrontendService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Presentation;

public static class DependencyInjection
{
	public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration config)
	{
		services.Configure<MailAppSettings>(config.GetSection("MailAppSettings"));

		services.AddApplication()
			.AddInfrastructure(config);

		services.AddScoped<MultiService>();
		return services;
	}
}