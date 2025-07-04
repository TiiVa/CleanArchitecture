using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.DTOs.Converters;

public static class InvitationConvertTo
{
    public static InvitationDto ConvertToDto(this Invitation m)
    {
        var d = new InvitationDto()
        {
            Id = m.Id,
            ContactEmail = m.ContactEmail,
            ContactInfo = m.ContactInfo,
            CreatedAt = m.CreatedAt,
            EventEndAt = m.EventEndAt,
            EventIntroduction = m.EventIntroduction,
            EventLocation = m.EventLocation,
            EventStartAt = m.EventStartAt,
            FileFormat = m.FileFormat,
            FileName = m.FileName,
            RegisterUrl = m.RegisterUrl,
            ShowWelcomeText = m.ShowWelcomeText,
            UpdatedAt = m.UpdatedAt,
            UpdatedByUser = m.UpdatedByUser

        };

        if (m.PictureFile is not null)
        {
            d.PictureFile = m.PictureFile;
        }

        d.Sections = m.Sections.Select(x => new InvitationSectionDto()
        {
            Id = x.Id,
            HyperLink = x.HyperLink,
            HyperLinkHeader = x.HyperLinkHeader,
            SectionHeader = x.SectionHeader,
            SectionDisplayNumber = x.SectionDisplayNumber,
            SectionBody = x.SectionBody,
            InvitationId = m.Id
        }).ToList();

        return d;
    }


    public static Invitation ConvertToInvitationForSave(InvitationDto d)
    {
     
        var m = new Invitation()
        {
            Id = d.Id,
            EventStartAt = d.EventStartAt,
            EventEndAt = d.EventEndAt,
            EventIntroduction = d.EventIntroduction,
            PictureFile = d.PictureFile,
            FileName = d.FileName,
            FileFormat = d.FileFormat,
            EventLocation = d.EventLocation,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            UpdatedByUser = d.UpdatedByUser,
            ContactInfo = d.ContactInfo,
            ContactEmail = d.ContactEmail,
            RegisterUrl = d.RegisterUrl,
            ShowWelcomeText = d.ShowWelcomeText,
        };

        if (d.Events is not null)
        {
            m.Events = d.Events.Select(x => new Event()
            {
                Id = x.EventId,
                Name = x.EventName,
                Description = x.EventDescription,
                EventDate = x.EventDate
            }).ToList();
        }
        if (d.Sections is null) return m;

        m.Sections = d.Sections.Select(x => new InvitationSection()
        {
            Id = x.Id,
            HyperLink = x.HyperLink,
            HyperLinkHeader = x.HyperLinkHeader,
            SectionHeader = x.SectionHeader,
            SectionDisplayNumber = x.SectionDisplayNumber,
            SectionBody = x.SectionBody,
            InvitationId = m.Id
        }).ToList();

        return m;
    }

    public static Invitation ConvertToInvitation(InvitationDto d)
    {

        var m = new Invitation()
        {
            Id = d.Id,
            EventStartAt = d.EventStartAt,
            EventEndAt = d.EventEndAt,
            EventIntroduction = d.EventIntroduction,
            PictureFile = d.PictureFile,
            FileName = d.FileName,
            FileFormat = d.FileFormat,
            EventLocation = d.EventLocation,
            CreatedAt = d.CreatedAt,
            UpdatedAt = d.UpdatedAt,
            UpdatedByUser = d.UpdatedByUser,
            ContactInfo = d.ContactInfo,
            ContactEmail = d.ContactEmail,
            RegisterUrl = d.RegisterUrl,
            ShowWelcomeText = d.ShowWelcomeText,
        };

        if (d.Events is not null)
        {
            m.Events = d.Events.Select(x => new Event()
            {
                Id = x.EventId,
                Name = x.EventName,
                Description = x.EventDescription,
                EventDate = x.EventDate
            }).ToList();
        }
        if (d.Sections is null) return m;

        m.Sections = d.Sections.Select(x => new InvitationSection()
        {
            Id = x.Id,
            HyperLink = x.HyperLink,
            HyperLinkHeader = x.HyperLinkHeader,
            SectionHeader = x.SectionHeader,
            SectionDisplayNumber = x.SectionDisplayNumber,
            SectionBody = x.SectionBody,
            InvitationId = m.Id
            
        }).ToList();

        return m;
    }

}