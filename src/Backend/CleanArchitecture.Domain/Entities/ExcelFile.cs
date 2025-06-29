using CleanArchitecture.Domain.Entities.Interfaces;

namespace CleanArchitecture.Domain.Entities;

public class ExcelFile : IEntity<Guid>
{
	public Guid Id { get; set; } = Guid.NewGuid();

	public string Path { get; set; } = string.Empty;

	public string ApplicationUserId { get; set; } = string.Empty;

	public ApplicationUser ApplicationUser { get; set; } = null!;
}