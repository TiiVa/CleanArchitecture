using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Presentation.FrontendService;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;

namespace CleanArchitecture.Client.Components.Pages.AdminPages;

public partial class InvitationLinkedToCreateEvent : ComponentBase
{

    [Inject] AuthenticationStateProvider AuthenticationStateProvider { get; set; }
    [Inject] MultiService MultiService { get; set; }
    [Inject] NavigationManager NavigationManager { get; set; }

    [Parameter]
    public string? EventIdFromParameter { get; set; }
    private InvitationDto Invitation { get; set; } = null!;
    private List<int> SectionsWithWrongHyperlinkFormat { get; set; } = new();
    private bool ShowInvitationPreview { get; set; }
    private bool ShowSaveOptionsModal { get; set; }
    private bool IsValidInvitation { get; set; }
    private string MailTo { get; set; } = string.Empty;
    
    protected override void OnInitialized()
    {
        CreateNewInstanceOfTypes();
    }

    #region InvitationSetup

    private void CreateNewInstanceOfTypes()
    {
        Invitation = new InvitationDto
        {
            Sections = new List<InvitationSectionDto>(),
            EventStartAt = DateTime.Today,
            EventEndAt = DateTime.Today,
            ShowWelcomeText = true
        };

        MailTo = string.Empty;
        SectionsWithWrongHyperlinkFormat.Clear();
    }

    private void AddSection()
    {
        Invitation.Sections.Add(new InvitationSectionDto() { SectionHeader = "Agenda" });
    }

    private async void RemoveSection(Guid sectionId)
    {
        var sectionToRemove = Invitation.Sections.FirstOrDefault(x => x.Id == sectionId);
        if (sectionToRemove != null)
        {
            Invitation.Sections.Remove(sectionToRemove);

            CheckAllSectionsForIncorrectHyperlinkFormat();
        }

        return;
    }

    private void ResetForm()
    {
       CreateNewInstanceOfTypes();
    }
    
    #endregion

    #region SaveInvitation

    private async Task SaveInvitation()
    {
        var userInfo = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var userEmail = userInfo.User.Identity?.Name;
        Invitation.UpdatedByUser = userEmail ?? "Unknown. Caused in Server.Invitations.razor";
        
        await SaveNewInvitationToDb();
    }

    private async Task SaveNewInvitationToDb()
    {

        var invitationId = Invitation.Id;

        var response = await MultiService.InvitationService.CreateInvitation(Invitation);

        if (response.IsFailed)
        {
            IsValidInvitation = false;
        }

        CloseSaveOptionsModal();
        CreateNewInstanceOfTypes();
        await MergeEventAndInvitation(invitationId);
        
    }

    private async Task MergeEventAndInvitation(Guid invitationId)
    {
        var invitationToMerge = await MultiService.InvitationService.GetInvitationById(invitationId);
        if (invitationToMerge.IsFailed || invitationToMerge.Value is null)
        {
            return;
        }

        var eventToMerge = await MultiService.EventService.GetAllEvents();
        if (eventToMerge.IsFailed || !eventToMerge.Value.Any())
        {
            return;
        }

        var eventFromParameter = eventToMerge.ValueOrDefault.FirstOrDefault(x => x.EventId == Guid.Parse(EventIdFromParameter));

        eventFromParameter.InvitationId = invitationToMerge.Value.Id;


        eventFromParameter.EventRegistrationForms.Clear();

        var response = await MultiService.EventService.UpdateEvent(eventFromParameter, eventFromParameter.EventId);

        if (response.IsFailed)
        {
           return;
        }

        RedirectToEventPage();
    }

    private void RedirectToEventPage()
    {
        NavigationManager.NavigateTo("/Events?showModal=true");
    }
    

    #endregion

    #region CheckHyperLinkFormat

    ///  <summary>
    /// Exists to inform user hyperlink is in incorrect format while granting freedom to save invitation in current state
    /// - replace with data annotation if saving is not a requirement.
    /// </summary>
    private void CheckHyperlinkForIncorrectFormat(ChangeEventArgs? e, int index)
    {
        if (e?.Value is not null)
            Invitation.Sections[index].HyperLink = e.Value.ToString()!;

        if (HyperlinkIsEmptyOrCorrectFormat(Invitation.Sections[index].HyperLink))
        {
            SectionsWithWrongHyperlinkFormat.Remove(index);
            return;
        }

        var found = SectionsWithWrongHyperlinkFormat.Exists(x => x.Equals(index));
        if (found) return;

        SectionsWithWrongHyperlinkFormat.Add(index);
        SectionsWithWrongHyperlinkFormat = SectionsWithWrongHyperlinkFormat.Order().ToList();
    }

    private static bool HyperlinkIsEmptyOrCorrectFormat(string hyperLink)
    {
        hyperLink = hyperLink.ToLower();

        return hyperLink.StartsWith("http://") ||
               hyperLink.StartsWith("https://") ||
               hyperLink.StartsWith("ftp://") ||
               string.IsNullOrWhiteSpace(hyperLink);
    }

    private string GetSectionNumbersWithWrongHyperlinkFormat()
    {
        return string.Join(" & ", SectionsWithWrongHyperlinkFormat.Select(sect => sect + 1).ToList());
    }

    private void CheckAllSectionsForIncorrectHyperlinkFormat()
    {
        SectionsWithWrongHyperlinkFormat.Clear();
        for (int i = 0; i < Invitation.Sections.Count; i++)
        {
            CheckHyperlinkForIncorrectFormat(null, i);
        }
    }

    private bool IsRegisterUrlWrongFormat()
    {
        return !HyperlinkIsEmptyOrCorrectFormat(Invitation.RegisterUrl);
    }

    #endregion

    #region Open&CloseModals
   
    private void OpenPreviewInvitation(InvitationDto? inv)
    {
        ShowInvitationPreview = true;
        if (inv is not null)
            Invitation = inv;
        MailTo = $"mailto:{Invitation.ContactEmail}?subject=Användarmöte";
    }

    private void ClosePreviewInvitation()
    {
        ShowInvitationPreview = false;
        MailTo = string.Empty;
    }

    private void CloseSaveOptionsModal()
    {
        ShowSaveOptionsModal = false;
    }

    #endregion

    #region Image

    private async Task HandleFileUpload(InputFileChangeEventArgs e)
    {
        if (e?.File is null || e.File.Size == 0) return;

        Invitation.FileName = e.File.Name;
        Invitation.FileFormat = e.File.ContentType;

        // 10 000 000 bytes == 10 MB
        await using var stream = e.File.OpenReadStream(10000000);
        using var memoryStream = new MemoryStream();

        await stream.CopyToAsync(memoryStream);
        Invitation.PictureFile = memoryStream.ToArray();
    }

    private void DeletePictureDataFromInvitation()
    {
        Invitation.PictureFile = null;
        Invitation.FileFormat = string.Empty;
        Invitation.FileName = string.Empty;
    }

    #endregion
}