using CleanArchitecture.Application.Interfaces.RepositoryInterfaces;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Repositories;

public class StoryRepository : IStoryRepository
{
    private readonly ApplicationDbContext _dbContext;

    public StoryRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task<IEnumerable<Story>> GetAllAsync()
    {
        var storyList = await _dbContext.Stories.Include(user => user.ApplicationUser).ToListAsync();
        return storyList;
    }

    public async Task<bool> AddAsync(Story entity)
    {
        var objectToSave = _dbContext.Stories.Add(entity);
        if (objectToSave is null)
        {
            return false;
        }

        return true;
    }

    public async Task<bool> UpdateAsync(Story entity, Guid id)
    {
        var storyToUpdate = await _dbContext.Stories.FirstOrDefaultAsync(s => s.Id == id);

        if (storyToUpdate is null)
        {
            return false;
        }
        storyToUpdate.ActiveStory = entity.ActiveStory;
        storyToUpdate.PublicStory = entity.PublicStory;
        storyToUpdate.StoryText = entity.StoryText;
        storyToUpdate.StoryHeading = entity.StoryHeading;
        storyToUpdate.EventId = entity.EventId;
        _dbContext.Entry(storyToUpdate).State = EntityState.Modified;//Checks if the entity has changed.
        return true;

    }

    public async Task<bool> RemoveAsync(Guid id)
    {
        var objectToRemove = await _dbContext.Stories.FirstOrDefaultAsync(s => s.Id == id);
        if (objectToRemove is null)
        {
            return false;
        }
        _dbContext.Stories.Remove(objectToRemove);
        return true;


    }

    public async Task<Story> GetById(Guid id)
    {
        var story = await _dbContext.Stories.Include(user => user.ApplicationUser)
            .FirstOrDefaultAsync(s => s.Id == id);
        if (story is null)
        {
            return null;

        }
        return story;
    }

    public async Task<List<Story>> GetActive()
    {
        var activeStories = await _dbContext.Stories.Where(s => s.ActiveStory).ToListAsync();
        if (activeStories is null || !activeStories.Any())
        {
            return null;
        }
        return activeStories;
      

    }

    public async Task<List<Story>> GetPublic()
    {
        var publicStories = await _dbContext.Stories.Where(s => s.ActiveStory && s.PublicStory).ToListAsync();
        if (publicStories is null || !publicStories.Any())
        {
            return null;
        }
        return publicStories;
       
    }
}