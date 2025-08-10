using CleanArchitecture.Application.Interfaces.RepositoryInterfaces;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Repositories;

public class EventRepository : IEventRepository
{
    private readonly ApplicationDbContext _dbContext;
    public EventRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<IEnumerable<Event>> GetAllAsync()
    {
        var eventList = await _dbContext.Events
            .Include(e => e.Stories)
            .Include(e => e.ApplicationUser)
            .Include(e => e.Invitation)
            .ThenInclude(e => e.Sections)
            .Include(e => e.EventRegistrationForms)
            .Include(e => e.EventResources)
            .ThenInclude(e => e.Resource).ToListAsync();
        return eventList;
    }
    public async Task<bool> AddAsync(Event entity)
    {
        var objectToSave = await _dbContext.Events.AddAsync(entity);

        if (objectToSave.Entity != null)
        {
            return true;
        }

        return false;
    }
    public async Task<bool> UpdateAsync(Event entity, Guid id)
    {
        var newEvent = await _dbContext.Events.FirstOrDefaultAsync(e => e.Id == id);

        if (newEvent != null)
        {
            newEvent.Name = entity.Name;
            newEvent.Description = entity.Description;
            newEvent.OpenForRegistration = entity.OpenForRegistration;
            newEvent.IsVisibleToUser = entity.IsVisibleToUser;
            newEvent.EventDate = entity.EventDate;
            newEvent.EventType = entity.EventType;
            newEvent.EventRegistrationForms = entity.EventRegistrationForms;
            newEvent.EventResources = entity.EventResources;
            newEvent.Stories = entity.Stories;
            newEvent.ApplicationUserId = entity.ApplicationUserId;
            newEvent.ApplicationUser = entity.ApplicationUser;
            newEvent.Invitation = entity.Invitation;
            return true;
        }
        return false;
    }
    public async Task<bool> RemoveAsync(Guid id)
    {
        var objectToRemove = await _dbContext.Events.FirstOrDefaultAsync(e => e.Id == id);

        if (objectToRemove != null)
        {
            _dbContext.Events.Remove(objectToRemove);
            return true;
        }

        return false;

    }
    public async Task<Event> GetById(Guid id)
    {
        var objectToFind = await _dbContext.Events
            .Include(e => e.Stories)
            .Include(e => e.ApplicationUser)
            .Include(e => e.Invitation)
            .ThenInclude(e => e.Sections)
            .Include(e => e.EventRegistrationForms)
            .Include(e => e.EventResources)
            .ThenInclude(e => e.Resource)
            .ThenInclude(e => e.ApplicationUser)
            .FirstOrDefaultAsync(e => e.Id.Equals(id));

        if (objectToFind != null)
        {
            return objectToFind;
        }

        return null;

    }
    public async Task<List<Event>> GetEventForUserById(Guid id)
    {
        var eventForUser = await _dbContext
            .Events
            .Include(x => x.EventRegistrationForms)
            .Include(x => x.EventResources)
            .ThenInclude(x => x.Resource)
            .ThenInclude(x => x.Documents)
            .Where(e => e.IsVisibleToUser && e.Id.Equals(id)).ToListAsync();
        if (eventForUser != null)
        {
            return eventForUser;
        }

        return null;
    }
    public async Task<List<Event>> GetEventsOpenForRegistrationAsync()
    {
        var eventList = await _dbContext.Events
             .Include(e => e.Invitation)
             .Where(i => i.OpenForRegistration).ToListAsync();

        if (eventList != null)
        {
            return eventList;
        }

        return null;

    }
    public async Task<List<Event>> GetEventsVisibleToUserAsync()
    {
        var visibleEventsForUsers = await _dbContext.Events.Where(e => e.IsVisibleToUser).ToListAsync();

        if (visibleEventsForUsers != null)
        {
            return visibleEventsForUsers;
        }

        return null;
    }

}