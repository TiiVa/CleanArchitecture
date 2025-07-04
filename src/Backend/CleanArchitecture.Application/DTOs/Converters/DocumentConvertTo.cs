using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.DTOs.Converters;

public static class DocumentConvertTo
{
    public static DocumentDto ConvertToDto(this Document m)
    {
        var d = new DocumentDto()
        {
            DocumentId = m.Id,
            ResourceId = m.ResourceId,
            FileName = m.Name,
            File = m.File,
            FileType = m.Type,
            Description = m.Description,
            CreatedBy = m.ApplicationUser.Email ?? "unknown",
            Resource = new ResourceDto()
            {
                ResourceName = m.Resource.ResourceName,
                ResourceId = m.Resource.Id
            }
        };

        return d;
    }

    public static Document ConvertToModel(this DocumentDto d)
    {
        var m = new Document()
        {
            Id = d.DocumentId,
            ResourceId = d.ResourceId,
            Name = d.FileName,
            File = d.File,
            Type = d.FileType,
            Description = d.Description,
            ApplicationUser = new ApplicationUser(),
            Resource = new Resource()
            {
                ResourceName = d.Resource.ResourceName,
                Id = d.Resource.ResourceId
            }

            
        };

        return m;
    }

    public static Document ConvertToModelWithoutObjects(this DocumentDto d) 
    {
        var m = new Document()
        {
            Id = d.DocumentId,
            ResourceId = d.ResourceId,
            Name = d.FileName,
            File = d.File,
            Type = d.FileType,
            Description = d.Description
        };

        return m;
    }
}