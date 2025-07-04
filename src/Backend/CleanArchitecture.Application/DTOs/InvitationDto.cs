using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Application.DTOs;

public class InvitationDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required(ErrorMessage = "Event picture file is required")]
    public byte[]? PictureFile { get; set; } = null;
    [StringLength(maximumLength: 100)]
    public string FileName { get; set; } = string.Empty;
    [StringLength(maximumLength: 100)]
    public string FileFormat { get; set; } = string.Empty;
    [Required(ErrorMessage = "Event introduction is required")]
    [StringLength(maximumLength: 2000)]
    public string EventIntroduction { get; set; } = string.Empty;
    [Required(ErrorMessage = "Event location is required")]
    [StringLength(maximumLength: 2000)]
    public string EventLocation { get; set; } = string.Empty;
    [Required(ErrorMessage = "Event start date is required")]
    public DateTime EventStartAt { get; set; }
    [Required(ErrorMessage = "Event end date is required")]
    public DateTime EventEndAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string UpdatedByUser { get; set; } = string.Empty;
    [Required(ErrorMessage = "Contact e-mail is required", AllowEmptyStrings = false)]
    [StringLength(maximumLength: 2000)]
    [EmailAddress]
    public string ContactEmail { get; set; } = string.Empty;
    [Required(ErrorMessage = "Contact info is required")]
    [StringLength(maximumLength: 2000)]
    public string ContactInfo { get; set; } = string.Empty;

    [StringLength(maximumLength: 2000)]
    public string RegisterUrl { get; set; } = string.Empty;
    [Required(ErrorMessage = "Show welcome text is required")]
    public bool ShowWelcomeText { get; set; }
    
    public List<InvitationSectionDto?> Sections { get; set; } = new List<InvitationSectionDto>();
    
    public List<EventDto?> Events { get; set; } = new List<EventDto>();
}