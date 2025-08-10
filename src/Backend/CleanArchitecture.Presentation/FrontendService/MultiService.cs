using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Application.Interfaces.ServiceInterfaces;

namespace CleanArchitecture.Presentation.FrontendService;


public class MultiService 
{
    private readonly  IServiceFactory _serviceFactory;
    private readonly Dictionary<Type, object> _services;

    private readonly IExternalServiceFactory _externalServiceFactory;
    private readonly Dictionary<Type, object> _externalServices;

    public MultiService(IServiceFactory serviceFactory, IExternalServiceFactory externalServiceFactory)
    {
        _serviceFactory = serviceFactory;
        _services = new Dictionary<Type, object>();

        _externalServiceFactory = externalServiceFactory;
        _externalServices = new Dictionary<Type, object>();
    }

    public TService GetAccessToService<TService>() where TService : class
    {
        if (_services.TryGetValue(typeof(TService), out var existingService))
        {
            return (TService)existingService;
        }

        var service = _serviceFactory.CreateService<TService>();
        _services[typeof(TService)] = service;
        return service;
    }

    #region UserServices
    public IStoryService StoryService => GetAccessToService<IStoryService>();
    public IResourceService ResourceService => GetAccessToService<IResourceService>();
    public IInvitationService InvitationService => GetAccessToService<IInvitationService>();
    public IEventService EventService => GetAccessToService<IEventService>();
    public IDocumentService DocumentService => GetAccessToService<IDocumentService>();
    public IApplicationUserService ApplicationUserService => GetAccessToService<IApplicationUserService>();
    public IEventRegistrationFormService UserMeetingRegistrationFormService =>
        GetAccessToService<IEventRegistrationFormService>();
    #endregion
    
    public TService GetAccessToExternalService<TService>() where TService : class
    {
        if (_externalServices.TryGetValue(typeof(TService), out var existingService))
        {
            return (TService)existingService;
        }

        var service = _externalServiceFactory.CreateExternalService<TService>();
        _externalServices[typeof(TService)] = service;
        return service;
    }

    #region ExternalServices
    public IExcelExportService ExcelExportService => GetAccessToExternalService<IExcelExportService>();
    
    #endregion
    
}