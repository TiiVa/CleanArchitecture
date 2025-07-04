using CleanArchitecture.Application.DTOs.UserDtos;
using CleanArchitecture.Domain.Attribute;
using CleanArchitecture.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Application.DTOs;

public class EventRegistrationFormDto
{
	public Guid Id { get; set; } = Guid.NewGuid();

	[Required(ErrorMessage = "First name is missing.")]
	[TableColumn(columnHeading: "First Name", columnId: 1)]
	public string FirstName { get; set; } = null!;

	[Required(ErrorMessage = "Last name is missing.")]
	[TableColumn(columnHeading: "Last Name", columnId: 2)]
	public string FamilyName { get; set; } = null!;

	[Required(ErrorMessage = "Email is missing.")]
	[EmailAddress(ErrorMessage = "Email is in incorrect format.")]
	[TableColumn(columnHeading: "Email", columnId: 4)]
	public string Email { get; set; } = null!;

	[TableColumn(columnHeading: "Phone Number", columnId: 5)]
	public string PhoneNumber { get; set; } = string.Empty;

	[Required(ErrorMessage = "Company name is missing.")]
	[TableColumn(columnHeading: "Company", columnId: 3)]
	public string Company { get; set; } = null!;

	[TableColumn(columnHeading: "Invoice Reference", columnId: 13)]
	public string InvoiceReference { get; set; } = string.Empty;

	[TableColumn(columnHeading: "Accommodation", columnId: 6)]
	public Accommodation Accommodation { get; set; }

	[TableColumn(columnHeading: "AccommodationWith", columnId: 7)]
	public string AccommodationWith { get; set; } = string.Empty;

	[TableColumn(columnHeading: "Allergies", columnId: 8)]
	public string Allergies { get; set; } = string.Empty;

	[TableColumn(columnHeading: "UserInformation", columnId: 9)]
	public string UserInformation { get; set; } = string.Empty;

	[Required]
	[Range(typeof(bool), "true", "true", ErrorMessage = "To be able to participate in this event, we need permission to save your personal data.")]
	[TableColumn(columnHeading: "AgreeToSavePersonalData", columnId: 10)]
	public bool AgreeToSavePersonalData { get; set; }

	public EventDto Event { get; set; }

	[TableColumn(columnHeading: "Deltagare bekräftat", columnId: 12)]
	public bool Confirmed { get; set; }
}