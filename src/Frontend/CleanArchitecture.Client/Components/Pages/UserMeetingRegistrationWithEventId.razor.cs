using CleanArchitecture.Application.DTOs.UserDtos;
using CleanArchitecture.Presentation.FrontendService;
using Microsoft.AspNetCore.Components;

namespace CleanArchitecture.Client.Components.Pages;

public partial class UserMeetingRegistrationWithEventId : ComponentBase
{
    [Inject] private MultiService MultiService { get; set; }
    private EventRegistrationSparseDto UserMeetingRegistrationSparseDto { get; set; } = new();
    private List<EventInvitationInfoForRegistrationDto> EventsOpenForRegistration { get; set; } = new();

    [Parameter]
    public Guid EventId { get; set; }
    private bool RegistrationSubmitted { get; set; }
    private bool ShowMessageModal { get; set; }
    private bool IsLoading { get; set; }
    private bool SubmitButtonLocked { get; set; }
    private string MailTo { get; set; } = string.Empty;
    private string ModalMessage { get; set; } = string.Empty;

    #region GDPR info

    private const string GdprInfo =
		"EventSphere AB är personuppgiftsansvarig för behandlingen av de personuppgifter du lämnar till oss i samband med din anmälan till och deltagande av tillställningen. " +
        "Behandlingen sker i syfte att registrera, genomgång och vid behov fakturera, eller för att skicka e-post relaterade till tillställningen." +
        "\r\nOm det ingår någon typ av mat/dryck i samband med tillställningen, behöver vi också behandla uppgifter om matpreferenser i det fall du anger sådana. " +
        "Den rättsliga grunden för behandlingen är fullgörande av avtal. I förekommande fäll när vi utvärderar och/eller följer upp tillställningen är den rättsliga grunden legitimt intresse." +
        "\r\nOm du har frågor avseende vår behandling av dina personuppgifter eller om du vill utöva sina rättigheter enligt dataskyddsförordningen (GDPR), " +
        "kan du vända dig till vårt dataskyddsombud via e-post eller via vår hemsida. " +
        "Du har rätt att inge klagomål till Integritetsskyddsmyndigheten (IMY), vars kontaktuppgifter finns på www.imy.se.";

    #endregion

    protected override async Task OnInitializedAsync()
    {
        await GetEventAsync();
    }

    private async Task GetEventAsync()
    {
        IsLoading = true;

        try
        {
            var events = await MultiService.EventService.GetEventsOpenForRegistration();

            var currentEvent = events.Value.FirstOrDefault(e => e.EventId == EventId);

            UserMeetingRegistrationSparseDto.Event = currentEvent;

            EventsOpenForRegistration = events.Value;

        }
        catch (Exception ex)
        {
            OpenMessageModal(ex.Message);
            IsLoading = false;
            return;
        }

        if (UserMeetingRegistrationSparseDto.Event is null)
        {
            IsLoading = false;
            return;
        }

        MailTo = $"mailto:{EventsOpenForRegistration[0].ContactEmail}?subject=Användarmöte";

        IsLoading = false;
    }

    private void SelectedEventChanged(ChangeEventArgs e)
    {
        if (!Guid.TryParse(e.Value?.ToString(), out var id))
            return;

        var eventDto = EventsOpenForRegistration.FirstOrDefault(x => x.EventId.Equals(id));
        if (eventDto is null) return;

        UserMeetingRegistrationSparseDto.Event = eventDto;

        MailTo = $"mailto:{eventDto.ContactEmail}?subject=Användarmöte";
    }

    private async Task SubmitRegistration()
    {
        SubmitButtonLocked = true;

        var response = await MultiService.UserMeetingRegistrationFormService.AddAsync(UserMeetingRegistrationSparseDto);

        if (response.IsFailed)
        {
            OpenMessageModal(response.Errors.FirstOrDefault()?.Message ?? "Unknown error");
        }

        SubmitButtonLocked = false;

        SubmitButtonLocked = false;
        RegistrationSubmitted = true;
    }

    //private void GetErrorMessageTwo(string error)
    //{
    //    switch (error)
    //    {
    //        case "FailedUserAlreadyRegistered":
    //            OpenMessageModalWithError("User is already registered to event.");
    //            break;
    //        case "FailedNoEvent":
    //            OpenMessageModalWithError("Failed to find event.");
    //            break;
    //        case "FailedCreateAccount":
    //            OpenMessageModalWithError("Failed to create account.");
    //            break;
    //        default:
    //            OpenMessageModalWithError("An unexpected error occurred");
    //            break;

    //    }
    //}



    private void OpenMessageModalWithError(string result)
    {
        if (result.Equals("User is already registered to event."))
        {
            OpenMessageModal($"Användare {UserMeetingRegistrationSparseDto.Email} är redan anmäld till evenemanget." +
                             $"\n\nVid frågor kontakta oss " +
                             $"\n{UserMeetingRegistrationSparseDto.Event.ContactEmail}" +
                             $"\n{UserMeetingRegistrationSparseDto.Event.ContactInfo}");
            return;
        }

        if (result.Equals("Failed to find event.") || result.Equals("Failed to create account."))
        {
            OpenMessageModal(
                "Webbservern stötte på ett oväntat fel som hindrade den från att fullfölja begäran. Vänligen försök igen.");
            return;
        }

        OpenMessageModal(
            "Webbservern stötte på ett oväntat fel som hindrade den från att fullfölja begäran. Vänligen försök igen.");
    }

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

    private async Task RegisterAnotherParticipant()
    {
        RegistrationSubmitted = false;
        IsLoading = true;
        StateHasChanged();

        var company = UserMeetingRegistrationSparseDto.Company;

        UserMeetingRegistrationSparseDto = new()
        {
            Event = new(Guid.Empty, "", new DateTime(), "", ""),
            Company = company
        };

        await GetEventAsync();
    }
}