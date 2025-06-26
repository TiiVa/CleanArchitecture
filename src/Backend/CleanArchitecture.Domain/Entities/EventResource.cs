namespace CleanArchitecture.Domain.Entities;

public class EventResource
{
	public Guid EventId { get; set; }

	public Event Event { get; set; } = null!;

	public Guid ResourceId { get; set; }
	public Resource Resource { get; set; } = null!;
}