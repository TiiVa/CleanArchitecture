using CleanArchitecture.Application.DTOs.UserDtos;
using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CleanArchitecture.Application.DTOs.Converters;

public static class EventRegistrationFormConvertTo
{
	public static EventRegistrationFormDto ConvertToDto(this EventRegistrationForm m)
	{
		var d = new EventRegistrationFormDto()
		{
			Id = m.Id,
			Accommodation = m.Accommodation,
			UserInformation = m.UserInformation,
			PhoneNumber = m.PhoneNumber,
			AccommodationWith = m.AccommodationWith,
			AgreeToSavePersonalData = m.AgreeToSavePersonalData,
			Allergies = m.Allergies,
			Company = m.Company,
			Confirmed = m.Confirmed,
			Email = m.Email,
			FamilyName = m.LastName,
			FirstName = m.FirstName,
			InvoiceReference = m.InvoiceReference,
			Event = new EventDto() { EventId = m.EventId, EventName = m.Event.Name, EventDate = m.Event.EventDate }
		};

		return d;
	}

	public static EventRegistrationForm ConvertToModel(this EventRegistrationFormDto d)
	{
		return new EventRegistrationForm()
		{
			Id = d.Id,
			Event = new Event() { Id = d.Event.EventId },
			FirstName = d.FirstName,
			LastName = d.FamilyName,
			Email = d.Email,
			PhoneNumber = d.PhoneNumber,
			Company = d.Company,
			Accommodation = d.Accommodation,
			AccommodationWith = d.AccommodationWith,
			AgreeToSavePersonalData = d.AgreeToSavePersonalData,
			Allergies = d.Allergies,
			InvoiceReference = d.InvoiceReference,
			UserInformation = d.UserInformation,
			Confirmed = d.Confirmed,
		};
	}

	public static EventRegistrationFormDto ConvertToDto(this EventRegistrationSparseDto s)
	{
		return new EventRegistrationFormDto()
		{
			Event = new EventDto() { EventId = s.Event.EventId },
			FirstName = s.FirstName,
			FamilyName = s.FamilyName,
			Email = s.Email,
			PhoneNumber = s.PhoneNumber,
			Company = s.Company,
			Accommodation = s.Accommodation,
			AccommodationWith = s.AccommodationWith,
			AgreeToSavePersonalData = s.AgreeToSavePersonalData,
			Allergies = s.Allergies,
			InvoiceReference = s.InvoiceReference,
			UserInformation = s.UserInformation
		};
	}
}