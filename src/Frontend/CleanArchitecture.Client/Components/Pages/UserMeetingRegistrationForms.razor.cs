using CleanArchitecture.Application.DTOs.UserDtos;
using CleanArchitecture.Presentation.FrontendService;
using Microsoft.AspNetCore.Components;

namespace CleanArchitecture.Client.Components.Pages;

public partial class UserMeetingRegistrationForms : ComponentBase
{
    [Inject] private MultiService MultiService { get; set; }
    private EventRegistrationSparseDto UserMeetingRegistrationSparseDto { get; set; } = new();
    private List<EventInvitationInfoForRegistrationDto> EventsOpenForRegistration { get; set; } = new();

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
        await GetEventsAsync();
    }

    private async Task GetEventsAsync()
    {
        IsLoading = true;

        var events = await MultiService.EventService.GetEventsOpenForRegistration();

        if (events.IsFailed)
        {
            IsLoading = false;
            OpenMessageModal(events.Errors.FirstOrDefault()?.Message ?? "Unknown error");
            return;
        }

        var validEvents = events.Value.Where(e => e.EventDate >= DateTime.Today);

        EventsOpenForRegistration = validEvents.OrderBy(x => x.EventDate).ToList();

        if (!EventsOpenForRegistration.Any())
        {
            IsLoading = false;
            return;
        }

        UserMeetingRegistrationSparseDto.Event = EventsOpenForRegistration[0];

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

        await GetEventsAsync();
    }
}