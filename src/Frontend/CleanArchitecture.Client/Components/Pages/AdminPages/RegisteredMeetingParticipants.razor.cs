using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Presentation.FrontendService;
using Microsoft.AspNetCore.Components;

namespace CleanArchitecture.Client.Components.Pages.AdminPages;

public partial class RegisteredMeetingParticipants : ComponentBase
{
    [Inject] private MultiService MultiService { get; set; }
    private bool EventWithUsersExists => EventsWithRegisteredUsers.Any();
    private bool IsLoadingParticipants { get; set; }
    private bool ShowMessageModal { get; set; }
    private bool ShowUpdateParticipantModal { get; set; }

    private List<EventDto> EventsWithRegisteredUsers { get; set; } = new();
    private List<bool> ShowOrCollapseEventOnIndex { get; set; } = new();

    private string ModalMessage { get; set; } = string.Empty;

    private EventRegistrationFormDto Participant { get; set; } = new();


    protected override async Task OnInitializedAsync()
    {
        await GetAllUsersRegisteredForAnyEventFromDbAsync();
    }

    private async Task GetAllUsersRegisteredForAnyEventFromDbAsync()
    {
        IsLoadingParticipants = true;
        
        try
        {
            var participants = await MultiService.UserMeetingRegistrationFormService.GetAllAsync();

            if (participants.IsFailed || !participants.Value.Any())
            {
                IsLoadingParticipants = false;
                OpenMessageModal(participants.Errors.FirstOrDefault()?.Message ?? "Unknown error");
                return;
            }

            EventsWithRegisteredUsers = EventDto.GroupByEvent(participants.Value.ToList());
        }
        catch (Exception ex)
        {
            IsLoadingParticipants = false;
            OpenMessageModal($"Could not get participants. {ex.Message}");
            return;
        }

        ShowOrCollapseEventOnIndex = new List<bool>(EventsWithRegisteredUsers.Count);
        for (int i = 0; i < EventsWithRegisteredUsers.Count; i++)
        {
            ShowOrCollapseEventOnIndex.Add(false);
        }

        IsLoadingParticipants = false;
    }

    #region Update participant
    private void OpenUpdateParticipantModal(EventRegistrationFormDto participant)
    {
        Participant = participant;
        ShowUpdateParticipantModal = true;
    }

    private void CloseUpdateParticipantModal()
    {
        ShowUpdateParticipantModal = false;
    }

    private async Task UpdateParticipantInDbAsync()
    {

        try
        {
            var result = await MultiService.UserMeetingRegistrationFormService.UpdateFormAsync(Participant, Participant.Id);
            

            if (result.IsFailed)
            {
                OpenMessageModal(result.Errors.FirstOrDefault()?.Message ?? "Unknown error");
            }
        }
        catch (Exception e)
        {
            OpenMessageModal($"Could not update participant. {e.Message}");
            return;
        }

        ShowUpdateParticipantModal = false;

        await GetAllUsersRegisteredForAnyEventFromDbAsync();
    }
    #endregion

    private async Task DeleteParticipantInDbAsync(EventRegistrationFormDto participant)
    {
        
        try
        {
             
             var result = await MultiService.UserMeetingRegistrationFormService.DeleteFormAsync(participant.Id);
            if (result.IsFailed)
            {
                OpenMessageModal(result.Errors.FirstOrDefault()?.Message ?? "Unknown error");
            }
            
        }
        catch (Exception e)
        {
            OpenMessageModal($"Could not delete participant. {e.Message}");
            return;
        }

        EventsWithRegisteredUsers.Clear();
        await GetAllUsersRegisteredForAnyEventFromDbAsync();
    }

    #region Message modal
    private void OpenMessageModal(string message)
    {
        ShowMessageModal = true;
        ModalMessage = message;
    }

    private void CloseMessageModal()
    {
        ShowMessageModal = false;
        ModalMessage = "";
    }
    #endregion
}