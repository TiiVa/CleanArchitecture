using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Application.DTOs;

public class StoryDto
{
    public Guid StoryId { get; set; } = Guid.NewGuid();
    [Required(ErrorMessage = "Story text is required")]
    public string StoryText { get; set; } = string.Empty;
    [Required(ErrorMessage = "Story heading is required")]
    public string StoryHeading { get; set; } = string.Empty;

    public bool ActiveStory { get; set; }

    public bool PublicStory { get; set; }
    [Required(ErrorMessage = "Story create date is required")]
    public DateTime StoryCreated { get; set; }

    public DateTime? EventDate { get; set; }


    public string CreatedBy { get; set; } = string.Empty;

    public Guid? EventId { get; set; }
}