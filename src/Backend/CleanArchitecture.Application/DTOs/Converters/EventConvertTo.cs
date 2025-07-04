using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.DTOs.Converters;

public static class EventConvertTo
{
    public static EventDto ConvertToDto(this Event e)
    {
        var d = new EventDto
        {
            EventId = e.Id,
            EventDate = e.EventDate,
            EventDescription = e.Description,
            EventName = e.Name,
            EventType = e.EventType,
            OpenForRegistration = e.OpenForRegistration,
            IsVisibleToUser = e.IsVisibleToUser,
            CreatedBy = e.ApplicationUser?.Email ?? ""
        };

        if (e.Invitation is not null)
        {
            d.InvitationId = e.Invitation.Id;
        }

        d.Resources = e.EventResources.Select(x => new ResourceDto()
        {
            ResourceId = x.Resource.Id,
            CreatedBy = x.Resource.ApplicationUser.Email ?? "Unknown",
            CreatedAt = x.Resource.CreatedAt,
            Context = x.Resource.Context,
            ResourceDescription = x.Resource.ResourceDescription,
            ResourceName = x.Resource.ResourceName,
            ResourceType = x.Resource.ResourceType
        }).ToList();

        d.EventRegistrationForms = e.EventRegistrationForms.Select(x => x.ConvertToDto()).ToList();

        return d;
    }


}
