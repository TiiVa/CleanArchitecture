using System.Security.AccessControl;
using CleanArchitecture.Domain.Entities.Interfaces;

namespace CleanArchitecture.Domain.Entities;

public class Resource : IEntity<Guid>
{
	public Guid Id { get; set; } = Guid.NewGuid();

	public string ResourceName { get; set; } = string.Empty;

	public ResourceType ResourceType { get; set; }

	public string Context { get; set; } = string.Empty;

	public string ResourceDescription { get; set; } = string.Empty;

	public DateTime CreatedAt { get; set; }

	public string ApplicationUserId { get; set; } = string.Empty;
	public ApplicationUser ApplicationUser { get; set; }

	public ICollection<Document> Documents { get; set; } = new List<Document>();

	public ICollection<EventResource> EventResources { get; set; } = new List<EventResource>();
}