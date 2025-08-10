using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.DTOs.Converters;
using CleanArchitecture.Application.DTOs.UserDtos;
using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Application.Interfaces.RepositoryInterfaces;
using CleanArchitecture.Application.Interfaces.ServiceInterfaces;
using CleanArchitecture.Domain.Entities;
using FluentResults;

namespace CleanArchitecture.Application.Services;

public class EventService : IEventService
{
    private readonly IUnitOfWork _uow;
    private readonly ISerilogLogger _logger;

    public EventService(IUnitOfWork uow, ISerilogLogger logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<Result> CreateEvent(EventDto? eventDto)
    {
        if (eventDto is null)
        {
            return Result.Fail("Eventet saknar värden");
        }

        try
        {
            var remadeEvent = new Event
            {
                EventDate = eventDto.EventDate,
                Name = eventDto.EventName,
                Description = eventDto.EventDescription,
                EventType = eventDto.EventType,
                OpenForRegistration = eventDto.OpenForRegistration,
                IsVisibleToUser = eventDto.IsVisibleToUser
            };

            var user = await _uow.CreateRepository<IApplicationUserRepository>().GetAllInfoByEmailAsync(eventDto.CreatedBy);

            remadeEvent.ApplicationUserId = user.Id;
            remadeEvent.ApplicationUser = user;

            if (eventDto.InvitationId != null)
            {
                remadeEvent.Invitation = await _uow.CreateRepository<IInvitationRepository>().GetByIdAsync(eventDto.InvitationId.Value);
            }

            var successObject = await _uow.CreateRepository<IEventRepository>().AddAsync(remadeEvent);

            if (!successObject)
            {
                _logger.LogInformation($"No Event was created.");
                return Result.Fail("Det gick inte att skapa eventet");
            }
            await _uow.CompleteAsync();
            _logger.LogInformation("Event created");

            return Result.Ok();
        }

        
        catch (Exception e)
        {
            _logger.LogError("Error", e);
            return Result.Fail(new Error("Ett oväntat fel inträffade").CausedBy(e));
        }
        

    }
    public async Task<Result<IEnumerable<EventDto>>> GetAllEvents()
    {

        try
        {
            var eventList = await _uow.CreateRepository<IEventRepository>().GetAllAsync();

            if (!eventList.Any())
            {
                _logger.LogInformation($"No Events retrieved due to empty list.");
                return Result.Fail("Inga event att visa.");
            }

            _logger.LogInformation($"Events retrieved: {eventList.Count()}");

            return Result.Ok(eventList.Select(e => e.ConvertToDto()));
        }
        catch (Exception e)
        {
            _logger.LogError("Error", e);
            return Result.Fail(new Error("Ett oväntat fel inträffade").CausedBy(e));
        }
    }
    public async Task<Result> UpdateEvent(EventDto eventDto, Guid id)
    {
        try
        {
            var eventToUpdate = await _uow.CreateRepository<IEventRepository>().GetById(id);

            if (eventToUpdate is null)
            {
                _logger.LogInformation("Event was not found.");
                return Result.Fail("Kunde inte hitta eventet.");
            }

            eventToUpdate.Name = eventDto.EventName;
            eventToUpdate.Description = eventDto.EventDescription;
            eventToUpdate.OpenForRegistration = eventDto.OpenForRegistration;
            eventToUpdate.IsVisibleToUser = eventDto.IsVisibleToUser;
            eventToUpdate.EventDate = eventDto.EventDate;
            eventToUpdate.EventType = eventDto.EventType;
            
            if (!eventDto.EventRegistrationForms.Any())
            {
                eventToUpdate.EventRegistrationForms.Clear();
            }
            
            var user = await _uow.CreateRepository<IApplicationUserRepository>().GetAllInfoByEmailAsync(eventDto.CreatedBy);
            eventToUpdate.ApplicationUser = user;
            eventToUpdate.ApplicationUserId = user.Id;


            eventToUpdate.EventResources.Clear();
            for (int i = 0; i < eventDto.Resources.Count; i++)
            {
                var resource = await _uow.CreateRepository<IResourceRepository>()
                    .GetByIdAsync(eventDto.Resources[i].ResourceId);
                if (resource is null)
                {
                    _logger.LogInformation($"Resource with ID:{eventDto.Resources[i].ResourceId} doesn't exist.");
                    break;
                }
                
                var duplicate = eventToUpdate.EventResources.Any(x => x.ResourceId == resource.Id);
                if (duplicate)
                {
                    _logger.LogInformation($"Resource with ID:{eventDto.Resources[i].ResourceId} is duplicated.");

                    break;
                }
                var eventResource = new EventResource
                {
                    EventId = eventDto.EventId,
                    ResourceId = resource.Id
                };

                eventToUpdate.EventResources.Add(eventResource);
            }
            
            if (eventDto.InvitationId is not null)
            {
                eventToUpdate.Invitation = await _uow.CreateRepository<IInvitationRepository>()
                    .GetByIdAsync(eventDto.InvitationId.Value);
            }
            else
            {
                eventToUpdate.Invitation = null;
            }
            
            var successObject = await _uow.CreateRepository<IEventRepository>().UpdateAsync(eventToUpdate, id);

            if (!successObject)
            {
                _logger.LogInformation($"Event with event id {id} could not be updated.");
                return Result.Fail($"Eventet med id {id} kunde inte uppdateras.");
            }

            await _uow.CompleteAsync();
            _logger.LogInformation($"Event successfully updated.");
            return Result.Ok();
        }
        catch (Exception e)
        {
            _logger.LogError("Error", e);
            return Result.Fail(new Error("Ett oväntat fel inträffade").CausedBy(e));
        }

    }
    public async Task<Result> DeleteEvent(Guid id)
    {
        try
        {
            var successObject = await _uow.CreateRepository<IEventRepository>().RemoveAsync(id);

            if (!successObject)
            {
                _logger.LogInformation($"No Events was Deleted with ID:{id}.");
                return Result.Fail($"Kunde inte ta bort event med id {id}");
            }
            await _uow.CompleteAsync();
            _logger.LogInformation($"Event successfully Deleted.");
            return Result.Ok();
        }
        catch (Exception e)
        {
            _logger.LogError("Error", e);
            return Result.Fail(new Error("Ett oväntat fel inträffade").CausedBy(e));
        }
        
    }
    public async Task<Result<EventDocumentsParticipantsDto>> GetEventForUser(Guid id)
    {
        
        try
        {
            var userToSearchFor = await _uow.CreateRepository<IEventRepository>().GetEventForUserById(id);
            var resultOfEvent = userToSearchFor.Select(e => new EventDocumentsParticipantsDto(
                    e.Name,
                    e.Description,
                    e.EventDate,
                    e.EventResources.SelectMany(eventResource => eventResource.Resource.Documents
                        .Select(document => new FileNoDataDto(document.Id, document.Name, document.Type))).ToList(),
                    e.EventRegistrationForms.Select(userRegistration => new ParticipantDto(
                        userRegistration.FirstName,
                        userRegistration.LastName, userRegistration.Email, userRegistration.Company)).ToList()))
                .FirstOrDefault();
            if (resultOfEvent is null)
            {
                _logger.LogInformation($"No Event found on User.");
                return Result.Fail("Inga event hittades.");

            }
            _logger.LogInformation($"Event found on User.");
            return Result.Ok(resultOfEvent);
        }
        catch (Exception e)
        {
            _logger.LogError("Error", e);
            return Result.Fail(new Error("Ett oväntat fel inträffade").CausedBy(e));
        }


    }
    public async Task<Result<List<EventInvitationInfoForRegistrationDto>>> GetEventsOpenForRegistration()
    {

        try
        {
            var eventsOpenForRegistration = await _uow.CreateRepository<IEventRepository>().GetEventsOpenForRegistrationAsync();

            var eventInvitationInfo = eventsOpenForRegistration.Select(x => new EventInvitationInfoForRegistrationDto(
                x.Id,
                x.Name,
                x.EventDate,
                x.Invitation != null ? x.Invitation.ContactEmail : "hello@eventsphere.com",
                x.Invitation != null ? x.Invitation.ContactInfo : "Anna Anderson, +46 701 11 22")).ToList();

            if (!eventInvitationInfo.Any() || eventInvitationInfo.Count == 0)
            {
                _logger.LogInformation($"No Event found for registration.");
                return Result.Fail("Inga event öppna för registrering hittades.");
            }
            
            _logger.LogInformation($"Event found for registration.");
            return Result.Ok(eventInvitationInfo);
        }
        catch (Exception e)
        {
            _logger.LogError("Error", e);
            return Result.Fail(new Error("Ett oväntat fel inträffade").CausedBy(e));
        }
      

    }
    public async Task<Result<List<EventSparseInfoDto>>> GetEventsVisibleToUser()
    {

        try
        {
            var visibleEvents = await _uow.CreateRepository<IEventRepository>().GetEventsVisibleToUserAsync();
            var filterEvent = visibleEvents
                .Where(x => x.IsVisibleToUser)
                .Select(x => new EventSparseInfoDto(x.Id, x.Name, x.EventDate)).ToList();

            if (!filterEvent.Any() || filterEvent.Count == 0)
            {
                _logger.LogInformation($"No Events visible for user found.");
                return Result.Fail("Inga event hittades.");
            }
            _logger.LogInformation($"Events visible for user found.");

            return Result.Ok(filterEvent);
        }
        catch (Exception e)
        {
            _logger.LogError("Error", e);
            return Result.Fail(new Error("Ett oväntat fel inträffade").CausedBy(e));

        }
      

    }
}



