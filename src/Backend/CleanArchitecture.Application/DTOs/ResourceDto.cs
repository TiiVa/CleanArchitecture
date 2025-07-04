using System.ComponentModel.DataAnnotations;
using CleanArchitecture.Domain.Enums;

namespace CleanArchitecture.Application.DTOs;

public class ResourceDto : ICloneable
{
    public Guid ResourceId { get; set; } = Guid.NewGuid();
    [Required(ErrorMessage = "Resource name is required")]
    public string ResourceName { get; set; } = string.Empty;
    [Required(ErrorMessage = "Resource type is required")]
    public ResourceType ResourceType { get; set; }
    public string Context { get; set; } = string.Empty;
    [Required(ErrorMessage = "Resource description is required")]
    public string ResourceDescription { get; set; } = string.Empty;

    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public int DocumentCount { get; set; } // New property to hold the count of documents if list of document is unpopulated

    public ICollection<DocumentDto> Documents { get; set; } = new List<DocumentDto>();

    public ICollection<EventDto> Events { get; set; } = new List<EventDto>();


    /// <returns>A new ResourceDto object that is a deep copy of this instance.</returns>
    public object Clone()
    {
        return new ResourceDto()
        {
            ResourceId = ResourceId,
            ResourceName = ResourceName,
            ResourceType = ResourceType,
            Context = Context,
            ResourceDescription = ResourceDescription,
            CreatedBy = CreatedBy,
            CreatedAt = CreatedAt,
            Documents = Documents,
            Events = Events,
            DocumentCount = DocumentCount
        };
    }
}