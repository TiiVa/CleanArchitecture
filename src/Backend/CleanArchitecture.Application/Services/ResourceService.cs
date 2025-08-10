using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.DTOs.Converters;
using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Application.Interfaces.RepositoryInterfaces;
using CleanArchitecture.Application.Interfaces.ServiceInterfaces;
using FluentResults;

namespace CleanArchitecture.Application.Services;

public class ResourceService : IResourceService
{
    private readonly IUnitOfWork _uow;
    private readonly ISerilogLogger _logger;

    public ResourceService(IUnitOfWork uow, ISerilogLogger logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<ResourceDto>>> GetAllResources()
    {
        try
        {
            var allResources = await _uow.CreateRepository<IResourceRepository>().GetAllAsync();

            if (!allResources.Any())
            {
                _logger.LogInformation("No Resources were retrieved.");
                return Result.Fail("Inga resurser hittades.");
            }
            _logger.LogInformation("Resources were retrieved.");
            return Result.Ok(allResources.Select(r => r.ConvertToDto(r.Documents.Count)));
        }
        catch (Exception e)
        {
           _logger.LogError("Error", e);
            return Result.Fail(new Error("Ett oväntat fel har inträffat.").CausedBy(e));
        }

    }
    
    public async Task<Result> AddResourceAsync(ResourceDto resource)
    {
        try
        {
            var user = await _uow.CreateRepository<IApplicationUserRepository>().GetAllInfoByEmailAsync(resource.CreatedBy);
            if (user is null)
            {
                _logger.LogInformation("No User found in resource.");
                return Result.Fail("Användare saknas.");
            }

            var resourceModel = resource.ConvertToModel();
            resourceModel.ApplicationUser = user;
            resourceModel.CreatedAt = DateTime.UtcNow;

            var success = await _uow.CreateRepository<IResourceRepository>().AddAsync(resourceModel);
            if (!success)
            {
                _logger.LogInformation("Could not Add resource");
                return Result.Fail("Kunde inte skapa ny resurs.");
            }
            await _uow.CompleteAsync();
            _logger.LogInformation("Resource Added.");

            return Result.Ok();

        }
        catch (Exception e)
        {
           _logger.LogError("Error", e);
           return Result.Fail(new Error("Ett oväntat fel har inträffat.").CausedBy(e));
        }
        
    }

    public async Task<Result> UpdateResourceAsync(ResourceDto resource, Guid id)
    {
        try
        {
            var user = await _uow.CreateRepository<IApplicationUserRepository>().GetAllInfoByEmailAsync(resource.CreatedBy);
            if (user is null)
            {
                _logger.LogInformation("No User found in resource.");
                return Result.Fail("Användare saknas.");
            }

            var resourceEntity = resource.ConvertToModel();
            resourceEntity.ApplicationUser = user;
            var success = await _uow.CreateRepository<IResourceRepository>().UpdateAsync(resourceEntity, id);

            if (!success)
            {
                _logger.LogInformation("Resource Not updated.");
                return Result.Fail("Kunde inte uppdatera resurs.");
            }
            await _uow.CompleteAsync();
            _logger.LogInformation("Resource updated.");
            return Result.Ok();

        }
        catch (Exception e)
        {
            _logger.LogError("Error", e);
            return Result.Fail(new Error("Ett oväntat fel har inträffat.").CausedBy(e));
        }
       

    }

    public async Task<Result> DeleteResourceAsync(Guid id)
    {
        try
        {
            var success = await _uow.CreateRepository<IResourceRepository>().RemoveAsync(id);

            if (!success)
            {
                _logger.LogInformation($"Could not Delete Resource with ID: {id}");
                return Result.Fail($"Kunde inte ta bort resurs med id {id}");
            }
            await _uow.CompleteAsync();
            _logger.LogInformation($"Resource deleted.");

            return Result.Ok();

        }
        catch (Exception e)
        {
            _logger.LogError("Error", e);
            return Result.Fail(new Error("Ett oväntat fel har inträffat.").CausedBy(e));
        }
 

    }
}