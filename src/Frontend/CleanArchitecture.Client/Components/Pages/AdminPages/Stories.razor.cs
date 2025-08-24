using System.Security.Claims;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Presentation.FrontendService;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace CleanArchitecture.Client.Components.Pages.AdminPages;

public partial class Stories : ComponentBase
{
    private bool showMessage;
    private bool showUpdateStory;

    [Inject] private MultiService MultiService { get; set; } = null!;
    [Inject] private AuthenticationStateProvider? AuthenticationStateProvider { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }
    public ClaimsPrincipal? ClaimsPrincipal { get; set; }
    public StoryDto? StoryToUpdate { get; private set; }
    public StoryDto OldStory { get; private set; }
    public string ModalMessage { get; private set; }
    public List<StoryDto>? StoryList { get; set; } = new();
    public List<EventDto>? EventList { get; private set; } = new();
    public StoryDto NewStory { get; private set; } = new();
    private bool showCreateStory;
    private bool loadingStories;
    private string? linkToEventPage;

    

    protected override async Task OnInitializedAsync()
    {
      
        await GetAllStoriesAsync();
        
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        ClaimsPrincipal = authState.User;
       
    }

    private Task ToggleModal()
    {
        showUpdateStory = !showUpdateStory;

        return Task.CompletedTask;
    }

    private async Task GetAllStoriesAsync()
    {
        try
        {
            loadingStories = true;

            var stories = await MultiService.StoryService.GetAllStories();

            if (stories.IsFailed || stories.Value.Count < 1)
            {
                loadingStories = false;
                ShowMessage(stories.Errors.FirstOrDefault()?.Message ?? "Unknown error");
                return;
            }

            StoryList = stories.Value;
            
            loadingStories = false;
            StateHasChanged();

        }
        catch (Exception ex)
        {
            loadingStories = false;
            ShowMessage($"Could not get Stories. {ex.Message}");
            StateHasChanged();
        }
    }

    private void ShowMessage(string message)
    {
        showMessage = true;
        ModalMessage = message;
    }

    private void CloseModalMessage()
    {
        showMessage = false;
        ModalMessage = "";
    }

    private async Task UpdateStory(Guid storyId)
    {

        try
        {
            await ToggleModal();

            if (EventList.Count == 0)
            {
                await GetAllEvents();
                
            }

            if (StoryList is not null && StoryList.Count > 0)
            {
                var story = await MultiService.StoryService.GetStoryById(storyId);

                StoryToUpdate = story.Value;

                if (StoryToUpdate != null)
                {
                    OldStory = new StoryDto()
                    {
                        StoryId = StoryToUpdate.StoryId,
                        StoryHeading = StoryToUpdate.StoryHeading,
                        StoryText = StoryToUpdate.StoryText,
                        StoryCreated = StoryToUpdate.StoryCreated,
                        ActiveStory = StoryToUpdate.ActiveStory,
                        PublicStory = StoryToUpdate.PublicStory,
                        CreatedBy = StoryToUpdate.CreatedBy,
                    };
                   
                   StateHasChanged();

                }
                else
                {
                    await ToggleModal();
                    ShowMessage($"Story not found in list.");
                }
            }
            else
            {
               
                await ToggleModal();
                ShowMessage($"No stories in list.");
            }
            
        }
        catch (Exception ex)
        {
            
            await ToggleModal();
            ShowMessage($"Could not update story. {ex.Message}");
        }
    }

    private async Task SaveUpdate()
    {
        try
        {
            await ToggleModal();

            if (StoryToUpdate is not null)
            {
                if (StoryToUpdate.ActiveStory is false)
                {
                    StoryToUpdate.PublicStory = false;
                }
                await MultiService.StoryService.UpdateStory(StoryToUpdate, StoryToUpdate.StoryId);

                StoryList.Clear();
                
                await OnInitializedAsync();
                StateHasChanged();

            }
            else
            {
                await ToggleModal();
                ShowMessage("Could not update story");
                StateHasChanged();
            }
            
        }
        catch (Exception ex)
        {
            await ToggleModal();
            ShowMessage($"Could not save story. {ex.Message}");
            StateHasChanged();
        }
    }
    private async Task GetAllEvents()
    {
        var result = await MultiService.EventService.GetAllEvents();

        if (result.IsFailed)
        {
            ShowMessage(result.Errors.FirstOrDefault()?.Message ?? "Unknown error");
            return;
        }

        var eventList = result.Value.Where(e => e.EventDate >= DateTime.Today).ToList();


        EventList.AddRange(eventList);
    }

    private async Task SaveStory()
    {
        var eventForStory = EventList.FirstOrDefault(e => e.EventId == NewStory.EventId);



        if (eventForStory.InvitationId is null || eventForStory.InvitationId == Guid.Empty || eventForStory.EventId == Guid.Empty || eventForStory.EventId == null)
        {
            ShowMessage($"Invitation for the event is missing. Go to Event page, Update the Event with an Invitation and try again.");
            showCreateStory = false;
            return;

        }

        showCreateStory = false;
        NewStory.CreatedBy = ClaimsPrincipal.Identity.Name;
        NewStory.StoryCreated = DateTime.Now;
        await MultiService.StoryService.CreateStory(NewStory);

        StoryList.Add(NewStory);
        NewStory = new StoryDto();
        StateHasChanged(); 
        
    }

    private async Task DeleteStory(Guid storyId)
    {
        try
        {
            if (StoryList is not null && StoryList.Count > 0)
            {
                StoryToUpdate = StoryList.FirstOrDefault(x => x.StoryId == storyId);

                if (StoryToUpdate.StoryId  != Guid.Empty)
                {
                    await MultiService.StoryService.DeleteStory(StoryToUpdate.StoryId);

                    StoryList.Clear();
                    await GetAllStoriesAsync();
                    StateHasChanged();

                }
                else
                {
                    ShowMessage("Could not delete story.");
                    StateHasChanged();
                }
             
            }
            else
            {
                ShowMessage($"No stories in list.");
                StateHasChanged();
            }
           
        }
        catch (Exception ex)
        {
            ShowMessage($"Could not delete story. {ex.Message}");
            StateHasChanged();
        }
    }


    private async Task CancelUpdate()
    {
        try
        {
            
            await ToggleModal();
            if (StoryList != null)
            {
                var story = StoryList.FirstOrDefault(x => x.StoryId == OldStory.StoryId);
                if (story != null)
                {
                    story.StoryCreated = OldStory.StoryCreated;
                    story.StoryHeading = OldStory.StoryHeading;
                    story.StoryText = OldStory.StoryText;
                    story.PublicStory = OldStory.PublicStory;
                    story.ActiveStory = OldStory.ActiveStory;
                }
                else
                {
                    ShowMessage("Could not reset old story values. Refresh page.");
                }
            }
            else
            {
                ShowMessage("Could not reset old story values. Refresh page.");
            }
        }
        catch (Exception ex)
        {
            showUpdateStory = false;
            ShowMessage($"Could not cancel update story. {ex.Message}");
        }
    }

    private async Task CreateStory()
    {
        if (EventList is not null && EventList.Count == 0)
        {
            await GetAllEvents();
        }

        NewStory = new StoryDto();
        showCreateStory = true;
        
    }

    private void CancelCreateStory()
    {
        showCreateStory = false;
        NewStory = null;
    }

    private void ViewStoryText(Guid storyId)
    {
        var storyText = StoryList.FirstOrDefault(x => x.StoryId == storyId).StoryText;
        ShowMessage(storyText);
    }
    private void GoBackToEventPage()
    {
        NavigationManager.NavigateTo("/Events");

    }
}