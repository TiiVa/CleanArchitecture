using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Application.DTOs;

public class InvitationSectionDto
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid InvitationId { get; set; }

    public int SectionDisplayNumber { get; set; }

    [StringLength(maximumLength: 2000)]
    public string SectionHeader { get; set; } = string.Empty;

    [StringLength(maximumLength: 2000)]
    public string SectionBody { get; set; } = string.Empty;

    [StringLength(maximumLength: 2000)]
    public string HyperLink { get; set; } = string.Empty;

    [StringLength(maximumLength: 2000)]
    public string HyperLinkHeader { get; set; } = string.Empty;
}