using System.ComponentModel.DataAnnotations.Schema;
using CleanArchitecture.Domain.Entities.Interfaces;
using CleanArchitecture.Domain.Enums;

namespace CleanArchitecture.Domain.Entities;

public class EventRegistrationForm : IEntity<Guid>
{
	public Guid Id { get; set; } = Guid.NewGuid();

	public string FirstName { get; set; } = string.Empty;

	public string LastName { get; set; } = string.Empty;

	public string Company { get; set; } = string.Empty;

	public string InvoiceReference { get; set; } = string.Empty;

	public string Email { get; set; } = string.Empty;

	public string PhoneNumber { get; set; } = string.Empty;

	public Accommodation Accommodation { get; set; }

	public string AccommodationWith { get; set; } = string.Empty;

	public string Allergies { get; set; } = string.Empty;

	public string UserInformation { get; set; } = string.Empty;

	public bool AgreeToSavePersonalData { get; set; }

	public bool Confirmed { get; set; }

	public Guid EventId { get; set; }
	public Event Event { get; set; }

	[NotMapped]
	public List<EventRegistrationForm> ParticipantRegistrations { get; set; } = [];
}