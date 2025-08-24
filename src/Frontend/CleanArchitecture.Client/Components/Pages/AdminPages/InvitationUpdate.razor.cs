using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Presentation.FrontendService;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;

namespace CleanArchitecture.Client.Components.Pages.AdminPages;

public partial class InvitationUpdate : ComponentBase
{
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; }
    [Inject] private MultiService MultiService { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }
    [Parameter] public Guid InvitationId { get; set; }
    [Parameter] public Guid EventId { get; set; }
    private InvitationDto Invitation { get; set; } = null!;
    private List<int> SectionsWithWrongHyperlinkFormat { get; set; } = new();
    private bool ShowInvitationPreview { get; set; }
    private string MailTo { get; set; } = string.Empty;
    private bool ShowInvitationToBeUpdated { get; set; }
    private bool ShowMessageModal { get; set; }
    private string ModalMessage { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await CreateNewInstanceOfTypes();

        ShowInvitationToBeUpdated = true;

    }

    #region InvitationSetup

    private async Task CreateNewInstanceOfTypes()
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

        await GetInvitationToUpdate();


    }

    private async Task GetInvitationToUpdate()
    {
        var invitationToUpdate = await MultiService.InvitationService.GetInvitationById(InvitationId);

        Invitation = invitationToUpdate.Value;
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
            await UpdateExistingInvitationInDb();
        }

        return;
    }


    #endregion

    #region SaveUpdateDeleteInvitation

    private async Task SaveInvitation()
    {
        var userInfo = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var userEmail = userInfo.User.Identity?.Name;
        Invitation.UpdatedByUser = userEmail ?? "Unknown. Caused in Server.Invitations.razor";

        await UpdateExistingInvitationInDb();
        NavigationManager.NavigateTo($"/Events?showUpdateModal=true&EventId={EventId}");

    }

    private async Task UpdateExistingInvitationInDb()
    {
        var response = await MultiService.InvitationService.UpdateInvitation(Invitation, Invitation.Id);

        if (response.IsFailed)
        {
            OpenMessageModal(response.Errors.FirstOrDefault()?.Message ?? "Unknown error");
            return;
        }

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
        ShowInvitationToBeUpdated = false;
        ShowInvitationPreview = true;
        if (inv is not null)
            Invitation = inv;
        MailTo = $"mailto:{Invitation.ContactEmail}?subject=Användarmöte";
    }

    private void ClosePreviewInvitation()
    {
        ShowInvitationPreview = false;
        ShowInvitationToBeUpdated = true;
        MailTo = string.Empty;
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

    private void GoBackToEventPage()
    {
        NavigationManager.NavigateTo("/Events");

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
}