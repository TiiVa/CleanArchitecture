using CleanArchitecture.Application.Interfaces.RepositoryInterfaces;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Repositories;

public class InvitationRepository : IInvitationRepository
{
    private readonly ApplicationDbContext _dbContext;

    public InvitationRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<bool> AddAsync(Invitation entity)
    {

        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        foreach (var invitationSection in entity.Sections)
        {
            invitationSection.Id = Guid.NewGuid();
        }

        var successObject = _dbContext.Invitations.Add(entity);
        if (successObject != null)
        {
            return true;
        }

        return false;
    }

    public async Task<bool> UpdateAsync(Invitation updatedInvitation, Guid id)
    {
        var invitationToUpdate =
            await _dbContext.Invitations.Include(x => x.Sections).FirstOrDefaultAsync(x => x.Id == id);
        
        if (invitationToUpdate is null)
        {
            return false;
        }

        invitationToUpdate.PictureFile = updatedInvitation.PictureFile;
        invitationToUpdate.FileName = updatedInvitation.FileName;
        invitationToUpdate.FileFormat = updatedInvitation.FileFormat;
        invitationToUpdate.EventIntroduction = updatedInvitation.EventIntroduction;
        invitationToUpdate.EventLocation = updatedInvitation.EventLocation;
        invitationToUpdate.EventStartAt = updatedInvitation.EventStartAt;
        invitationToUpdate.EventEndAt = updatedInvitation.EventEndAt;
        invitationToUpdate.UpdatedAt = DateTime.Now;
        invitationToUpdate.UpdatedByUser = updatedInvitation.UpdatedByUser;
        invitationToUpdate.ContactEmail = updatedInvitation.ContactEmail;
        invitationToUpdate.ContactInfo = updatedInvitation.ContactInfo;
        invitationToUpdate.RegisterUrl = updatedInvitation.RegisterUrl;
        invitationToUpdate.ShowWelcomeText = updatedInvitation.ShowWelcomeText;




        if (updatedInvitation.Sections is not null)
        {
            var sectionsToRemove = invitationToUpdate.Sections
                .Where(originalSection => !updatedInvitation.Sections
                    .Any(updatedSection => updatedSection.Id == originalSection.Id))
                .ToList();
            _dbContext.InvitationSections.RemoveRange(sectionsToRemove);

            foreach (var section in updatedInvitation.Sections)
            {
                if (section.Id == Guid.Empty)
                {
                    section.Id = Guid.NewGuid(); // Generera ett nytt unikt Id om det är tomt
                }

                section.InvitationId = invitationToUpdate.Id; // Sätt InvitationId

                var existingSection = invitationToUpdate.Sections
                    .FirstOrDefault(s => s.Id == section.Id);

                if (existingSection is null)
                {
                    // Lägg till ny sektion
                    _dbContext.InvitationSections.Add(section);
                    await _dbContext.SaveChangesAsync();
                   
                }
                else
                {
                    // Uppdatera befintlig sektion
                    existingSection.HyperLink = section.HyperLink;
                    existingSection.HyperLinkHeader = section.HyperLinkHeader;
                    existingSection.SectionHeader = section.SectionHeader;
                    existingSection.SectionDisplayNumber = section.SectionDisplayNumber;
                    existingSection.SectionBody = section.SectionBody;
                }
            }
        }
        _dbContext.Update(invitationToUpdate);
       

        return true;

    }
    public async Task<IEnumerable<Invitation>> GetAllAsync()
    {
        var invitations = await _dbContext.Invitations
            .Include(x => x.Events)
            .Include(x => x.Sections
                .OrderBy(s => s.SectionDisplayNumber))
            .ToListAsync();

        return invitations.ToList();
    }
    public async Task<Invitation> GetByIdAsync(Guid id)
    {
        var foundInvitation = await _dbContext.Invitations
            .Include(x => x.Events)
            .Include(x => x.Sections.OrderBy(s => s.SectionDisplayNumber))
            .FirstOrDefaultAsync(x => x.Id == id);

        if (foundInvitation is null)
        {
            return null;
        }

        return foundInvitation;
    }
    public async Task<bool> RemoveAsync(Guid id)
    {
        var invitationToRemove = await _dbContext.Invitations
            .Include(x => x.Sections.OrderBy(s => s.SectionDisplayNumber))
            .Include(x => x.Events)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (invitationToRemove is null)
        {
            return false;
        }

        //Severs relationship from dependent Events to allow deletion of Invitation.
        foreach (var anEvent in invitationToRemove.Events)
        {
            anEvent.Invitation = null;
        }

        _dbContext.Invitations.Remove(invitationToRemove);
        return true;
    }
    public async Task<List<Invitation>> GetInvitationsForEventsOpenForRegistrationAsync()
    {
        var invitations = await _dbContext.Events
            .Include(x => x.Invitation)
            .ThenInclude(x => x.Sections.OrderBy(s => s.SectionDisplayNumber))
            .Where(x => x.OpenForRegistration && x.Invitation != null)
            .Select(x => x.Invitation)
            .ToListAsync();

        if (invitations.Count == 0)
        {
            return null;
        }


        return invitations;
    }
}