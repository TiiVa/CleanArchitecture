using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Domain.Entities;

public class ApplicationUser : IdentityUser
{
	public ICollection<Story> Stories { get; set; } = new List<Story>();

	public ICollection<Resource> Resources { get; set; } = new List<Resource>();

	public ICollection<Document> Documents { get; set; } = new List<Document>();

	public ICollection<Event> Events { get; set; } = new List<Event>();
}