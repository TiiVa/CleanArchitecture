using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.DTOs.Converters;
using CleanArchitecture.Application.DTOs.UserDtos;
using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Application.Interfaces.RepositoryInterfaces;
using CleanArchitecture.Application.Interfaces.ServiceInterfaces;
using FluentResults;

namespace CleanArchitecture.Application.Services;

public class StoryService : IStoryService
{

    private readonly IUnitOfWork _uow;
    private readonly ISerilogLogger _logger;



    public StoryService(IUnitOfWork uow, ISerilogLogger logger)
    {
        _uow = uow;
        _logger = logger;
    }


    public async Task<Result> CreateStory(StoryDto story)
    {

        try
        {
            if (story is null)
            {
                _logger.LogInformation($"No Story found to created.");
                return Result.Fail("Nyheten har felaktiga värden.");
            }

            var remadeStory = story.MapToStory();

            if (remadeStory is null)
            {
                _logger.LogInformation($"No Story found to created.");
                return Result.Fail("Hittade ingen nyhet.");
            }

            var user = await _uow.CreateRepository<IApplicationUserRepository>().GetAllInfoByEmailAsync(story.CreatedBy);
            if (user is null)
            {
                _logger.LogInformation($"No User bound to that Story.");
                return Result.Fail("Ingen användare knuten till nyhet.");
            }
            remadeStory.ApplicationUser = user;
            remadeStory.ApplicationUser_Id = Guid.Parse(user.Id);

            var successObject = await _uow.CreateRepository<IStoryRepository>().AddAsync(remadeStory);

            if (!successObject)
            {
                _logger.LogInformation("No Story created.");
                return Result.Fail("Kunde inte skapa nyhet.");
            }
            await _uow.CompleteAsync();
            _logger.LogInformation("New Story created successfully.");
            return Result.Ok();
        }
        catch (Exception e)
        {
            _logger.LogError("Error", e);
            return Result.Fail(new Error("Ett oväntat fel har inträffat.").CausedBy(e));
        }

      
    }

    public async Task<Result<List<StoryDto>>> GetAllStories()
    {
        try
        {
            var allStories = await _uow.CreateRepository<IStoryRepository>().GetAllAsync();

            if (allStories == null || !allStories.Any())
            {
                _logger.LogInformation("No List of Stories retrieved.");
                return Result.Fail("Inga Stories att visa.");
            }
            List<StoryDto> storiesList = allStories.ToList().MapStoryListToStoryDtoList();

            _logger.LogInformation("List of Stories retrieved.");
            return Result.Ok(storiesList);
        }
        catch (Exception e)
        {
            _logger.LogError("Error", e);
            return Result.Fail(new Error("Ett oväntat fel har inträffat.").CausedBy(e));
        }
    }

    public async Task<Result<StoryDto>> GetStoryById(Guid id)
    {

        try
        {
            var story = await _uow.CreateRepository<IStoryRepository>().GetById(id);
            if (story == null)
            {
                _logger.LogInformation($"No Story with ID:{id} found.");
                return Result.Fail($"Ingen nyhet med id {id} hittades.");
            }
            var storyDto = story.MapToStoryDto();
            _logger.LogInformation($"Story with ID:{id} found.");
            return Result.Ok(storyDto);
        }
        catch (Exception e)
        {
            _logger.LogError("Error", e);
            return Result.Fail(new Error("Ett oväntat fel har inträffat.").CausedBy(e));
        }

       
    }

    public async Task<Result<List<StorySparseDto>>> GetActiveStories()
    {

        try
        {
            var allActiveStories = await _uow.CreateRepository<IStoryRepository>().GetActive();

            if (allActiveStories == null || !allActiveStories.Any())
            {

                _logger.LogInformation("No Stories marked as Active retrieved.");
                return Result.Fail("Inga aktiva nyheter att visa.");

            }
            List<StorySparseDto> sparseStoryList = new();

            foreach (var story in allActiveStories)
            {
                var eventForStory = await _uow.CreateRepository<IEventRepository>().GetById(story.EventId.Value);
                if (eventForStory.Invitation is null)
                {
                    var sparseToAdd = new StorySparseDto(
                        story.StoryHeading,
                        story.StoryText,
                        story.StoryCreated = DateTime.UtcNow,
                        story.EventId ?? Guid.Empty);

                    sparseStoryList.Add(sparseToAdd);
                }
                else
                {
                    var invitationDate = await _uow.CreateRepository<IInvitationRepository>()
                        .GetByIdAsync(eventForStory.Invitation.Id); // Sets the date of the Event here

                    var sparseToAdd = new StorySparseDto(
                        story.StoryHeading,
                        story.StoryText,
                        story.StoryCreated = invitationDate.EventStartAt,
                        story.EventId ?? Guid.Empty);

                    sparseStoryList.Add(sparseToAdd);

                }
            }
            _logger.LogInformation("Stories marked as Active retrieved.");
            return Result.Ok(sparseStoryList);
        }
        catch (Exception e)
        {
            _logger.LogError("Error", e);
            return Result.Fail(new Error("Ett oväntat fel har inträffat.").CausedBy(e));
        }

       
    }

    public async Task<Result<List<StorySparseDto>>> GetPublicStories()
    {
        try
        {
            var allPublicStories = await _uow.CreateRepository<IStoryRepository>().GetPublic();

            if (allPublicStories == null || !allPublicStories.Any())
            {
                _logger.LogInformation("No Stories marked as Public retrieved.");
                return Result.Fail("Inga publika nyheter att visa.");
            }
            List<StorySparseDto> sparseStoryList = new();

            foreach (var story in allPublicStories)
            {
                var eventForStory = await _uow.CreateRepository<IEventRepository>().GetById(story.EventId.Value);
                if (eventForStory.Invitation is null)
                {
                    var sparseToAdd = new StorySparseDto(
                        story.StoryHeading,
                        story.StoryText,
                        story.StoryCreated = DateTime.UtcNow,
                        story.EventId ?? Guid.Empty);

                    sparseStoryList.Add(sparseToAdd);
                }
                else
                {
                    var invitationDate = await _uow.CreateRepository<IInvitationRepository>()
                        .GetByIdAsync(eventForStory.Invitation.Id); // Sets the date of the Event here

                    var sparseToAdd = new StorySparseDto(
                        story.StoryHeading,
                        story.StoryText,
                        story.StoryCreated = invitationDate.EventStartAt,
                        story.EventId ?? Guid.Empty);

                    sparseStoryList.Add(sparseToAdd);


                }
                
            }
            _logger.LogInformation("Stories marked as Public retrieved.");
            return Result.Ok(sparseStoryList);
        }
        catch (Exception e)
        {
            _logger.LogError("Error", e);
            return Result.Fail(new Error("Ett oväntat fel har inträffat.").CausedBy(e)); ;
        }

     
    }

    public async Task<Result> UpdateStory(StoryDto story, Guid id)
    {

        try
        {
            var user = await _uow.CreateRepository<IApplicationUserRepository>().GetAllInfoByEmailAsync(story.CreatedBy);

            var remadeToStory = story.MapToStory();

            remadeToStory.ApplicationUser = user;
            remadeToStory.ApplicationUser_Id = Guid.Parse(user.Id);

            var updated = await _uow.CreateRepository<IStoryRepository>().UpdateAsync(remadeToStory, id);

            if (!updated)
            {
                _logger.LogInformation("No Story were updated.");
                return Result.Fail("Uppdateringen misslyckades.");
            }
            await _uow.CompleteAsync();
            _logger.LogInformation("Story were updated.");

            return Result.Ok();
        }
        catch (Exception e)
        {
            _logger.LogError("Error", e);
            return Result.Fail(new Error("Ett oväntat fel har inträffat.").CausedBy(e)); ;
        }

    }
    public async Task<Result> DeleteStory(Guid id)
    {
        try
        {
            var storyToDelete = await _uow.CreateRepository<IStoryRepository>().RemoveAsync(id);
            if (!storyToDelete)
            {
                _logger.LogInformation($"Could not delete Story with ID: {id}");
                return Result.Fail($"Kunde inte ta bort nyhet med id {id}");

            }
            await _uow.CompleteAsync();
            _logger.LogInformation($"Delete Story");

            return Result.Ok();
        }
        catch (Exception e)
        {
           _logger.LogError("Error", e);
           return Result.Fail(new Error("Ett oväntat fel har inträffat.").CausedBy(e)); ;
        }
        
    }
}