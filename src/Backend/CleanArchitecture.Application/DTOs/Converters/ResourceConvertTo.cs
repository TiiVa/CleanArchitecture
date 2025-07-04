using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.DTOs.Converters;

public static class ResourceConvertTo
{
    public static ResourceDto ConvertToDto(this Resource m)
    {
        var d = new ResourceDto()
        {
            ResourceId = m.Id,
            ResourceName = m.ResourceName,
            ResourceType = m.ResourceType,
            ResourceDescription = m.ResourceDescription,
            Context = m.Context,
            CreatedAt = m.CreatedAt,
            CreatedBy = m.ApplicationUser.Email ?? "Unknown",
            Documents = m.Documents.Select(x => x.ConvertToDto()).ToList(),
            Events = m.EventResources.Select(x => new EventDto()
            {
                EventId = x.Event.Id,
                EventName = x.Event.Name,
                EventDate = x.Event.EventDate,
                EventDescription = x.Event.Description,
                EventType = x.Event.EventType,
                OpenForRegistration = x.Event.OpenForRegistration,
                IsVisibleToUser = x.Event.IsVisibleToUser,
                CreatedBy = x.Event.ApplicationUser?.Email ?? "Unknown"
            }).ToList()

        };

        return d;
    }

    public static ResourceDto ConvertToDto(this Resource m, int amountOfDocuments)
    {
        var d = new ResourceDto()
        {
            DocumentCount = amountOfDocuments,
            ResourceId = m.Id,
            ResourceName = m.ResourceName,
            ResourceType = m.ResourceType,
            ResourceDescription = m.ResourceDescription,
            Context = m.Context,
            CreatedAt = m.CreatedAt,
            CreatedBy = m.ApplicationUser.Email ?? "Unknown",
            Documents = m.Documents.Select(x => x.ConvertToDto()).ToList(),
            Events = m.EventResources.Select(x => new EventDto()
            {
                EventId = x.Event.Id,
                EventName = x.Event.Name,
                EventDate = x.Event.EventDate,
                EventDescription = x.Event.Description,
                EventType = x.Event.EventType,
                OpenForRegistration = x.Event.OpenForRegistration,
                IsVisibleToUser = x.Event.IsVisibleToUser,
                CreatedBy = x.Event.ApplicationUser?.Email ?? "Unknown"
            }).ToList()

        };

        return d;
    }

    public static Resource ConvertToModel(this ResourceDto d)
    {
        var m = new Resource()
        {
            Id = d.ResourceId,
            ResourceName = d.ResourceName,
            ResourceType = d.ResourceType,
            ResourceDescription = d.ResourceDescription,
            Context = d.Context,
            CreatedAt = d.CreatedAt,
            Documents = d.Documents.Select(x => x.ConvertToModel()).ToList(),
        }; 

        return m;
    }
}