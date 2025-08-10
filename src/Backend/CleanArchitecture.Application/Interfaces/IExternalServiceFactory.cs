namespace CleanArchitecture.Application.Interfaces;

public interface IExternalServiceFactory
{
    TService CreateExternalService<TService>() where TService : class;
}