using CleanArchitecture.Application.DTOs;
using FluentResults;

namespace CleanArchitecture.Application.Interfaces.ServiceInterfaces;

public interface IResourceService
{
    Task<Result<IEnumerable<ResourceDto>>> GetAllResources();
    Task<Result> AddResourceAsync(ResourceDto resource);
    Task<Result> UpdateResourceAsync(ResourceDto resource, Guid id);
    Task<Result> DeleteResourceAsync(Guid id);
}