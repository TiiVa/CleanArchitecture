using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.DTOs.Converters;

public static class StoryConvertTo
{
    public static StoryDto MapToStoryDto(this Story createdStory)
    {
        if (createdStory == null)
        {
            return new StoryDto();
        }
        var storyDto = new StoryDto()
        {
            StoryId = createdStory.Id,
            StoryCreated = createdStory.StoryCreated,
            StoryHeading = createdStory.StoryHeading,
            StoryText = createdStory.StoryText,
            ActiveStory = createdStory.ActiveStory,
            PublicStory = createdStory.PublicStory,
            EventId = createdStory.EventId
        };
        if (createdStory?.ApplicationUser != null)
        {
            storyDto.CreatedBy = createdStory.ApplicationUser.UserName;
        }
        return storyDto;
    }

    public static Story MapToStory(this StoryDto createdStory)
    {
       
        if (createdStory == null)
        {
            return new Story();
        }
        var story = new Story()
        {
            Id = createdStory.StoryId,
            StoryCreated = createdStory.StoryCreated,
            StoryHeading = createdStory.StoryHeading,
            StoryText = createdStory.StoryText,
            ActiveStory = createdStory.ActiveStory,
            PublicStory = createdStory.PublicStory,
            EventId = createdStory.EventId
        };
        return story;
    }

    public static List<StoryDto> MapStoryListToStoryDtoList(this List<Story> stories)
    {
        List<StoryDto> storiesList = new List<StoryDto>();

        foreach (var story in stories)
        {
            StoryDto storyDto = MapToStoryDto(story);
            storiesList.Add(storyDto);
        }

        return storiesList;
    }

}