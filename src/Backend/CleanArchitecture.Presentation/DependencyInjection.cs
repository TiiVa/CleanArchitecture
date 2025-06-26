using CleanArchitecture.Application;
using CleanArchitecture.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Presentation;

public static class DependencyInjection
{
	public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration config)
	{
		services.AddApplication()
			.AddInfrastructure(config);
		return services;
	}
}