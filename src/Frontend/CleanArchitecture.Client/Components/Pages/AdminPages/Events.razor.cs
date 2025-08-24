using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Presentation.FrontendService;
using FluentResults;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;

namespace CleanArchitecture.Client.Components.Pages.AdminPages;

public partial class Events : ComponentBase
{
    #region Properties

    [Inject] MultiService? MultiService { get; set; }
    [Inject] AuthenticationStateProvider? AuthenticationStateProvider { get; set; }
    [Inject] NavigationManager NavigationManager { get; set; }
    [Parameter] public Guid? EventId { get; set; }

    private int AmountOfResourceSelectionRows { get; set; } = 1;
    private const string NoResourceSelectedName = "No document folder selected";
    private string MailTo { get; set; } = string.Empty;
    private string SendToInvitation { get; set; } = "/invitations";
    private string SendToStories { get; set; } = "/stories";


    private string ModalMessage { get; set; } = string.Empty;
    private bool ResetParticipantsWhenUpdate { get; set; }
    private bool ShowAddDocumentationModal { get; set; }
    private bool ShowSaveSettingsModal { get; set; }
    private bool ShowAddInvitationModal { get; set; }
    private bool ShowAllInvitationsModalForCreateEvent { get; set; }
    private bool ShowUpdateEventModal { get; set; }
    private bool ShowResourcesModal { get; set; }
    private bool ShowParticipantsModal { get; set; }
    private bool ShowCreateEventModal { get; set; }
    private bool ShowMessageModal { get; set; }
    private bool ShowAllInvitationsModal { get; set; }
    private bool ShowPreviewInvitation { get; set; }
    private bool IsLoadingEvents { get; set; }
    private bool ShowConfirmDeletionModal { get; set; }
    private EventDto EventDto { get; set; } = null!;
    private InvitationDto Invitation { get; set; } = new();
    private List<InvitationDto> Invitations { get; set; } = new();
    private List<EventDto> AllEvents { get; set; } = new();
    private List<ResourceDto> AllResources { get; set; } = new();

    public string? EventIdForTransfer { get; set; }

    private bool ShowPublishEventModal { get; set; }
    

    #endregion

    #region Startup

    protected override async Task OnInitializedAsync()
    {
        InitializeEventData();
        await GetAllEventsFromDbAsync();

        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
        ShowPublishEventModal = query["showModal"] == "true";

        ShowUpdateEventModal = query["showUpdateModal"] == "true";

        var queryParams = QueryHelpers.ParseQuery(uri.Query);
        if (queryParams.TryGetValue("EventId", out var eventIdString) &&
            Guid.TryParse(eventIdString, out var eventId))
        {
            EventId = eventId;

            if (EventId != Guid.Empty && AllEvents.Count > 0)
            {
                EventDto = AllEvents.FirstOrDefault(e => e.EventId == EventId);
            }
            
        }
    }

    private void InitializeEventData()
    {
        EventDto = new EventDto()
        {
            EventDate = DateTime.Today,
            Resources = new List<ResourceDto>() { new ResourceDto() },
            EventRegistrationForms = new List<EventRegistrationFormDto>(),
            Stories = new List<StoryDto>(),
        };
    }

    #endregion

    #region Service calls

    private async Task GetAllInvitationsFromDbAsync()
    {
        var response = await MultiService.InvitationService.GetAllInvitations();

        if (response.IsFailed || !response.Value.Any())
        {
            OpenMessageModal(response.Errors.FirstOrDefault()?.Message ?? "Unknown error");
            return;
        }

        Invitations = response.Value
            .Where(x => x.EventStartAt >= DateTime.Today)
            .OrderBy(x => x.EventStartAt)
            .ToList();

        var passed = response.Value
            .Where(x => x.EventStartAt < DateTime.Today)
            .OrderByDescending(x => x.EventStartAt)
            .ToList();

        Invitations.Add(new InvitationDto() { EventIntroduction = "SkipThisToShowWhichInvitationsHavePassed" });

        Invitations.AddRange(passed);
    }

    private async Task<Result<InvitationDto>> GetInvitationFromDbAsync(Guid invId)
    {

        try
        {
            var result = await MultiService.InvitationService.GetInvitationById(invId);

            if (result.IsFailed)
            {
                OpenMessageModal(result.Errors.FirstOrDefault()?.Message ?? "Unknown error");
            }

            var invitation = result.Value;

            if (invitation is null) return Result.Fail("Invitation is null.");

            Invitation = invitation;
            return Result.Ok(Invitation);

        }
        catch (Exception e)
        {
            OpenMessageModal($"Could not get invitation. \n{e.Message}");
            return null;
        }
        
    }

    private async Task GetAllEventsFromDbAsync()
    {
        try
        {
            IsLoadingEvents = true;

            var response = await MultiService.EventService.GetAllEvents();
            if (response.IsFailed || !response.Value.Any())
            {
                IsLoadingEvents = false;
                OpenMessageModal(response.Errors.FirstOrDefault()?.Message ?? "Unknown error");
                return;
            }
            AllEvents = response.Value.OrderByDescending(x => x.EventDate).ToList();

            IsLoadingEvents = false;
            StateHasChanged();
        }
        catch (Exception ex)
        {
            IsLoadingEvents = false;
            OpenMessageModal($"Could not get Events. {ex.Message}");
            StateHasChanged();
        }
    }

    private async Task GetAllResourcesFromDbAsync()
    {
        AllResources = new List<ResourceDto>();

        var response = await MultiService.ResourceService.GetAllResources();

        if (response.IsFailed || !response.Value.Any())
        {
            OpenMessageModal(response.Errors.FirstOrDefault()?.Message ?? "Unknown error");
            AllResources.Add(new ResourceDto() { ResourceName = NoResourceSelectedName });
            return;
        }

        AllResources.Add(new ResourceDto() { ResourceName = NoResourceSelectedName });
        AllResources.AddRange(response.Value);
    }

    #endregion

    #region OpenCloseModals

    private void OpenParticipantsModal(EventDto dto)
    {
        InitializeEventData();
        EventDto = dto;
        ShowParticipantsModal = true;
    }

    private void CloseParticipantModal()
    {
        ShowParticipantsModal = false;
    }

    private async Task OpenPreviewInvitation(Guid invitationId)
    {


        var result = await GetInvitationFromDbAsync(invitationId);

        if (result.IsFailed)
        {
            ClosePreviewInvitation();
            OpenMessageModal(result.Errors.FirstOrDefault()?.Message ?? "Unknown error");
            return;
        }
        ShowPreviewInvitation = true;
        Invitation = result.Value;
        MailTo = $"mailto:{Invitation.ContactEmail}?subject=Användarmöte";
    }

    private void ClosePreviewInvitation()
    {
        ShowPreviewInvitation = false;
        Invitation = new InvitationDto();
        MailTo = string.Empty;
    }

    private void SelectInvitationAndCloseModal(Guid id)
    {
        ShowAllInvitationsModal = false;

        EventDto.InvitationId = id;
    }
    
    private async Task OpenInvitationsModal()
    {
        ShowAllInvitationsModal = true;
        await GetAllInvitationsFromDbAsync();
    }

    private void OpenMessageModal(string message)
    {
        ShowMessageModal = true;
        ModalMessage = message;
    }

    private void CloseMessageModal()
    {
        ShowMessageModal = false;
        ModalMessage = string.Empty;

    }

    private void OpenResourcesModal(EventDto dto)
    {
        InitializeEventData();
        EventDto = dto;
        ShowResourcesModal = true;
    }

    private void CloseResourcesModal()
    {
        ShowResourcesModal = false;
    }

    #endregion

    #region CreateNewEvent

    private async Task OpenCreateEventModal()
    {
        InitializeEventData();
        ShowCreateEventModal = true;
    }

    private void CloseCreateEventModal()
    {
        ShowCreateEventModal = false;
        AmountOfResourceSelectionRows = 1;
    }

    private void SelectedResourceChanged(ChangeEventArgs e, int index)
    {
        if (!Guid.TryParse(e.Value?.ToString(), out var id))
            return;

        var resource = AllResources.FirstOrDefault(x => x.ResourceId.Equals(id));
        if (resource is null) return;

        EventDto.Resources[index] = resource;
    }

    private void IncreaseResourceListInEventDtoAndAddResourceSelectionRow()
    {
        EventDto.Resources.Add(new ResourceDto());
        AmountOfResourceSelectionRows++;
    }

    private async Task SaveEventToDbAsync()
    {
        var user = await AuthenticationStateProvider.GetAuthenticationStateAsync();

        EventDto.CreatedBy = user.User.Identity?.Name ?? "Unknown";

        var saveEventName = EventDto.EventName;
        var response = await MultiService.EventService.CreateEvent(EventDto);

        if (response.IsFailed)
        {
            OpenMessageModal(response.Errors.FirstOrDefault()?.Message ?? "Unknown error");
            return;
        }

        await GetAllEventsFromDbAsync();

        var eventToCatch = await MultiService.EventService.GetAllEvents();

        if (eventToCatch.IsFailed || !eventToCatch.Value.Any())
        {
            OpenMessageModal(eventToCatch.Errors.FirstOrDefault()?.Message ?? "Unknown error");
            return;
        }

        var eventToMove = eventToCatch.Value.FirstOrDefault(x => x.EventName == saveEventName);

        OpenModalForSaveSettings(eventToMove.EventId);
        CloseCreateEventModal();
    }

    #endregion

    #region UpdateEvent

    private void RemoveInvitationFromEventDto()
    {
        EventDto.InvitationId = null;
        StateHasChanged();
    }

    private async void RemoveDocumentationFromEventDto(int index)
    {
        if (index >= 0 && index < EventDto.Resources.Count)
        {
            EventDto.Resources.RemoveAt(index);
            AmountOfResourceSelectionRows = EventDto.Resources.Count;
            StateHasChanged();
        }
    }

    private async Task OpenUpdateEventModal(EventDto dto)
    {
        InitializeEventData();
        EventDto = (EventDto)dto.Clone();

        await GetAllResourcesFromDbAsync();

        ShowUpdateEventModal = true;

        AmountOfResourceSelectionRows = dto.Resources.Count;
        IncreaseResourceListInEventDtoAndAddResourceSelectionRow();
    }

    private void CloseUpdateEventModal()
    {
        ShowUpdateEventModal = false;
        AmountOfResourceSelectionRows = 1;
        ResetParticipantsWhenUpdate = false;
    }

    private void OpenUpdateDocumentationModal()
    {
        InitializeEventData();
        ShowAddDocumentationModal = true;
    }

    private async void EventSelectionChanged(ChangeEventArgs e)
    {
        if (Guid.TryParse(e.Value.ToString(), out Guid selectedEventId))
        {
            var eventDocumentation = AllEvents.FirstOrDefault(ev => ev.EventId == selectedEventId);
            EventDto = (EventDto)eventDocumentation.Clone();
            if (EventDto != null)
            {
                await GetAllResourcesFromDbAsync();
                AmountOfResourceSelectionRows = EventDto.Resources.Count;
                IncreaseResourceListInEventDtoAndAddResourceSelectionRow();
                StateHasChanged();
            }
        }
    }

    private void CloseUpdateDocumentationModel()
    {
        ShowAddDocumentationModal = false;
        StateHasChanged();
    }

    private bool IsResourceExistentInEvent(ResourceDto resource, int index)
    {
        return
            resource.ResourceId.Equals(EventDto.Resources[index].ResourceId) &&
            !resource.ResourceName.Equals(NoResourceSelectedName);
    }

    private async Task UpdateEventInDbAsync()
    {
        var user = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        EventDto.CreatedBy = user.User.Identity?.Name ?? "Unknown";

        if (ResetParticipantsWhenUpdate)
        {
            EventDto.EventRegistrationForms.Clear();
        }
           

        var response = await MultiService.EventService.UpdateEvent(EventDto, EventDto.EventId);

        if (response.IsFailed)
        {
            OpenMessageModal(response.Errors.FirstOrDefault()?.Message ?? "Unknown error");
            return;
        }

        CloseAddInvitationsModal();
        CloseUpdateEventModal();
        CloseUpdateDocumentationModel();
        ShowPublishEventModal = true;
        await GetAllEventsFromDbAsync();
    }

    #endregion

    #region DeleteEvent

    private async Task DeleteEventFromDbAsync(EventDto dto)
    {

        var storyList = await MultiService.StoryService.GetAllStories();

        if (storyList.IsSuccess)
        {
            var removeStory = storyList.Value.Where(s => s.EventId == dto.EventId).ToList();

            if (removeStory.Count >= 1)
            {
                foreach (var story in removeStory)
                {
                    await MultiService.StoryService.DeleteStory(story.StoryId);
                }
            }
            var response = await MultiService.EventService.DeleteEvent(dto.EventId);

            if (response.IsFailed)
            {
                OpenMessageModal(response.Errors.FirstOrDefault()?.Message ?? "Unknown error");
                return;
            }
            
            ShowConfirmDeletionModal = false;
            AllEvents.Clear();
            await GetAllEventsFromDbAsync();
            StateHasChanged();

        }
        else if (storyList.IsFailed)
        {
            var response = await MultiService.EventService.DeleteEvent(dto.EventId);

            if (response.IsFailed)
            {
                OpenMessageModal(response.Errors.FirstOrDefault()?.Message ?? "Unknown error");
                return;
            }

            ShowConfirmDeletionModal = false;
            AllEvents.Clear();
            await GetAllEventsFromDbAsync();
            StateHasChanged();

        }



    }

    private void OpenConfirmDeletionModal(EventDto dto)
    {
        ShowConfirmDeletionModal = true;
        EventDto = dto;
    }

    private void CloseConfirmDeletionModal()
    {
        ShowConfirmDeletionModal = false;
        EventDto = new EventDto();
    }


    #endregion

    #region Navigations

    private void GoToCreateInvitationForEvent()
    {
        NavigationManager.NavigateTo(SendToInvitation);
    }

    private void GoToCreateStoryForEvent()
    {
        NavigationManager.NavigateTo(SendToStories);
    }

    #endregion

    #region Navigation thru CreateEvent

    private void OpenModalForSaveSettings(Guid id)
    {
        ShowSaveSettingsModal = true;
        EventDto.EventId = id;
        EventIdForTransfer = id.ToString();
    }

    private void CloseModalForSaveSettings()
    {
        ShowSaveSettingsModal = false;
        EventDto = new EventDto();
    }
    private void CloseModalForSaveSettingsWithoutClearingEventDto()
    {
        ShowSaveSettingsModal = false;
    }

    private async Task OpenAddInvitationModal()
    {
        CloseModalForSaveSettingsWithoutClearingEventDto();
        ShowAddInvitationModal = true;
        await GetAllInvitationsFromDbAsync();
    }
    private async Task OpenAddInvitationToEventModal()
    {
        CloseAddInvitationsModal();
        ShowAllInvitationsModalForCreateEvent = true;
        await GetAllInvitationsFromDbAsync();
    }
    private void CloseAddInvitationsModal()
    {
        ShowAllInvitationsModal = false;
        ShowAllInvitationsModalForCreateEvent = false;
        ShowAddInvitationModal = false;
    }
    private void CloseAddInvitationsModalAndGoBack()
    {
        ShowAllInvitationsModal = false;
        ShowAllInvitationsModalForCreateEvent = false;
        ShowAddInvitationModal = true;
    }
    private void SelectInvitationAndCloseModalAndReturn(Guid id)
    {
        ShowAllInvitationsModalForCreateEvent = false;
        EventDto.InvitationId = id;
        ShowAddInvitationModal = true;
    }

    private void OpenCreateInvitationForEventModal()
    {
        CloseAddInvitationsModal();
        NavigationManager.NavigateTo($"/InvitationForEvent{EventIdForTransfer}");

    }
    #endregion


    private void ClosePublishEventModal()
    {
        ShowPublishEventModal = false;
    }

    private void RedirectToStories()
    {
        ShowPublishEventModal = false;
        NavigationManager.NavigateTo("/stories");
    }

    private async Task UpdateEventBeforeRedirectToUpdateInvitationPage(Guid eventId)
    {
        var allEvents = await MultiService.EventService.GetAllEvents();

        if (allEvents.IsFailed)
        {
            OpenMessageModal(allEvents.Errors.FirstOrDefault()?.Message ?? "Unknown error");
            return;
        }

        var eventToUpdate = allEvents.Value.FirstOrDefault(e => e.EventId == eventId);

        var successWhenUpdatingEvent = await MultiService.EventService.UpdateEvent(EventDto, eventToUpdate.EventId);

        if (successWhenUpdatingEvent.IsFailed)
        {
            OpenMessageModal(successWhenUpdatingEvent.Errors.FirstOrDefault()?.Message ?? "Unknown error");
            return;
        }

        var invitationId = eventToUpdate.InvitationId;

       var invitation = await MultiService.InvitationService.GetInvitationById(invitationId.Value);

       if (invitation.IsFailed)
       {
           OpenMessageModal(invitation.Errors.FirstOrDefault()?.Message ?? "Unknown error");
            return;
       }

       RedirectToUpdateInvitationPage(invitation.Value, eventToUpdate.EventId);
    }

    private void RedirectToUpdateInvitationPage(InvitationDto invitation, Guid eventId)
    {

        CloseUpdateEventModal();

        NavigationManager.NavigateTo($"/invitationUpdate/{invitation.Id}/{eventId}");

    }

    private void GoBackToEventPage()
    {
        NavigationManager.NavigateTo("/Events");

    }

}