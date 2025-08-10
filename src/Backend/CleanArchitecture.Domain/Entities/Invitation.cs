using System.ComponentModel.DataAnnotations;
using CleanArchitecture.Domain.Entities.Interfaces;

namespace CleanArchitecture.Domain.Entities;

public class Invitation : IEntity<Invitation>
{
	public Guid Id { get; set; } = Guid.NewGuid();

	public byte[]? PictureFile { get; set; }

	[StringLength(maximumLength: 100)]
	public string FileName { get; set; } = string.Empty;

	[StringLength(maximumLength: 100)]
	public string FileFormat { get; set; } = string.Empty;

	[StringLength(maximumLength: 2000)]
	public string EventIntroduction { get; set; } = string.Empty;

	[StringLength(maximumLength: 2000)]
	public string EventLocation { get; set; } = string.Empty;

	public DateTime EventStartAt { get; set; }

	public DateTime EventEndAt { get; set; }

	public DateTime CreatedAt { get; set; }

	public DateTime UpdatedAt { get; set; }

	[StringLength(maximumLength: 2000)]
	public string UpdatedByUser { get; set; } = string.Empty;

	[StringLength(maximumLength: 2000)]
	public string ContactEmail { get; set; } = string.Empty;

	[StringLength(maximumLength: 2000)]
	public string ContactInfo { get; set; } = string.Empty;

	[StringLength(maximumLength: 2000)]
	public string RegisterUrl { get; set; } = string.Empty;

	public bool ShowWelcomeText { get; set; }

	public ICollection<InvitationSection> Sections { get; set; } = new List<InvitationSection>();

	public ICollection<Event> Events { get; set; } = new List<Event>();
}