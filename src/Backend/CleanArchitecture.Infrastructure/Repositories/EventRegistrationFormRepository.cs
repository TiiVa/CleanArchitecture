using CleanArchitecture.Application.Interfaces.RepositoryInterfaces;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Repositories;

public class EventRegistrationFormRepository : IEventRegistrationFormRepository
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public EventRegistrationFormRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
      
        _userManager = userManager;
    }
    public async Task<IEnumerable<EventRegistrationForm>> GetAllAsync()
    {
        var forms = await _context.UserMeetingRegistrationForms
            .Include(x => x.Event)
            .ToListAsync();

        return forms;
    }

    public async Task<bool> AddAsync(EventRegistrationForm entity)
    {
        var newRegistration = await _context.UserMeetingRegistrationForms.AddAsync(entity);

        if (newRegistration is null) return false;

        return true;
    }

    public async Task<bool> UserIsAlreadyRegisteredToEvent(EventRegistrationForm user)
    {
        return await _context.UserMeetingRegistrationForms
            .AnyAsync(x =>
                x.Email.ToLower().Equals(user.Email.ToLower()) &&
                x.EventId.Equals(user.Event.Id));
    }

    public async Task<bool> ConfirmParticipationInEvent(string userId, string eventId) 
    {
        var user = await _userManager.FindByIdAsync(userId) ?? throw new ApplicationException("User not found.");

        var eventIdGuid = Guid.Parse(eventId);

        var userForm = await _context.UserMeetingRegistrationForms
                           .FirstOrDefaultAsync(x => x.Email == user.Email && x.EventId == eventIdGuid)
                       ?? throw new ApplicationException("User not found in event.");

        userForm.Confirmed = true;

        if (userForm.Confirmed)
        {
            return true;
        }

        return false;
    }

    public async Task<bool> UpdateAsync(EventRegistrationForm entity, Guid id) 
    {
        var formToUpdate = await _context.UserMeetingRegistrationForms.FirstOrDefaultAsync(f => f.Id == id);

        if (formToUpdate is null) return false;

        formToUpdate.FirstName = entity.FirstName;
        formToUpdate.LastName = entity.LastName;
        formToUpdate.Email = entity.Email;
        formToUpdate.PhoneNumber = entity.PhoneNumber;
        formToUpdate.Company = entity.Company;
        formToUpdate.InvoiceReference = entity.InvoiceReference;
        formToUpdate.Accommodation = entity.Accommodation;
        formToUpdate.AccommodationWith = entity.AccommodationWith;
        formToUpdate.Allergies = entity.Allergies;
        formToUpdate.UserInformation = entity.UserInformation;
        formToUpdate.Confirmed = entity.Confirmed;

        return true;
    }

    public async Task<bool> RemoveAsync(Guid id) 
    {
        var formToDelete = await _context.UserMeetingRegistrationForms.FirstOrDefaultAsync(f => f.Id == id);

        if (formToDelete is null) return false;

        _context.UserMeetingRegistrationForms.Remove(formToDelete);

        return true;
    }

    public async Task<List<EventRegistrationForm>> GetRegistrationInfoByUserIdAsync(Guid id) 
    {
        var email = await _context
            .Users
            .Where(x => x.Id.Equals(id.ToString()))
            .Select(x => x.Email)
            .FirstOrDefaultAsync();

        if (email is null)
        {
            return null;
        }

        var result = await _context
            .UserMeetingRegistrationForms
            .Include(x => x.Event)
            .ThenInclude(x => x.Invitation)
            .Where(x =>
                x.Event.EventDate >= DateTime.Today &&
                x.Email.ToLower().Equals(email.ToLower()))
            .ToListAsync();

        return result;
    }

}