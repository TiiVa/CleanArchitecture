using CleanArchitecture.Domain.Enums;

namespace CleanArchitecture.Application.DTOs;

public class DocumentDto : ICloneable
{
    public Guid DocumentId { get; set; } = Guid.NewGuid();

    public string NewFileName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public FileType FileType { get; set; }

    public byte[]? File { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string CreatedBy { get; set; } = string.Empty;

    public Guid ResourceId { get; set; }
    public ResourceDto Resource { get; set; }


    /// <returns>A new DocumentDto object that is a deep copy of this instance.</returns>
    public object Clone()
    {
        return new DocumentDto
        {
            DocumentId = DocumentId,
            Resource = Resource,
            CreatedBy = CreatedBy,
            File = File,
            FileName = FileName,
            FileType = FileType,
            ResourceId = ResourceId,
            Description = Description,
            NewFileName = NewFileName
        };
    }
}