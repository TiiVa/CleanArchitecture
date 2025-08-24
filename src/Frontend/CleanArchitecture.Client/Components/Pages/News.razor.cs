using CleanArchitecture.Application.DTOs.UserDtos;
using CleanArchitecture.Presentation.FrontendService;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace CleanArchitecture.Client.Components.Pages;

public partial class News : ComponentBase
{
    [Inject] private MultiService MultiService { get; set; }
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; }
    
    private bool IsLoadingStories { get; set; }
    private bool ShowMessageModal { get; set; }
    private string ModalMessage { get; set; } = string.Empty;
    private List<StorySparseDto> Stories { get; set; } = new();


    protected override async Task OnInitializedAsync()
    {
        IsLoadingStories = true;
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

        if (authState.User.Identity is not null && authState.User.Identity.IsAuthenticated)
        {
            await GetActiveStoriesAsync();
        }
        else
        {
            await GetPublicStoriesAsync();
        }
    }

    private async Task GetPublicStoriesAsync()
    {
        try
        {
            var stories = await MultiService.StoryService.GetPublicStories();

            if (stories.IsFailed)
            {
                IsLoadingStories = false;
                return;
            }
            List<StorySparseDto> activeList = new();


            var active = stories.Value.Where(x => x.Date >= DateTime.Today).ToList();

            if (active is null)
            {
                IsLoadingStories = false;
                return;
            }

            Stories = active;
            IsLoadingStories = false;
        }
        catch (Exception ex)
        {
            IsLoadingStories = false;
            OpenMessageModal($"Webbservern stötte på ett oväntat fel som hindrade den från att fullfölja begäran. {ex.Message}");
        }
    }

    private async Task GetActiveStoriesAsync()
    {
        try
        {
            var stories = await MultiService.StoryService.GetActiveStories();

            if (stories.IsFailed)
            {
                IsLoadingStories = false;
                return;
            }

            var active = stories.Value.Where(x => x.Date >= DateTime.Today).ToList();

            if (active is null)
            {
                IsLoadingStories = false;
                return;
            }

            Stories = active;
            IsLoadingStories = false;
        }
        catch (Exception ex)
        {
            OpenMessageModal($"Webbservern stötte på ett oväntat fel som hindrade den från att fullfölja begäran. {ex.Message}");
        }

    }

    private void OpenMessageModal(string message)
    {
        ShowMessageModal = true;
        ModalMessage = message;
    }

    private void CloseModalMessage()
    {
        ShowMessageModal = false;
        ModalMessage = "";
    }

    
}