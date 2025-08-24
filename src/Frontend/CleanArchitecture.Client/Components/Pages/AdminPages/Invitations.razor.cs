using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Presentation.FrontendService;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;

namespace CleanArchitecture.Client.Components.Pages.AdminPages;

public partial class Invitations : ComponentBase
{
    [Inject] AuthenticationStateProvider AuthenticationStateProvider { get; set; }
    [Inject] MultiService MultiService { get; set; }

    [Inject] NavigationManager NavigationManager { get; set; }

    private InvitationDto Invitation { get; set; } = null!;
    private InvitationDto? InvitationToBeDeleted { get; set; }
    private List<InvitationDto> ManyInvitations { get; set; } = new();
    private List<int> SectionsWithWrongHyperlinkFormat { get; set; } = new();
    private bool ShowInvitationPreview { get; set; }
    private bool ShowInvitationsModal { get; set; }
    private bool ShowConfirmDeletionModal { get; set; }
    private bool ShowSaveOptionsModal { get; set; }
    private bool CanUpdateExistingInvitation { get; set; }
    private bool IsValidInvitation { get; set; }
    private string MailTo { get; set; } = string.Empty;

    private string SelectedColor { get; set; }
    private List<string> Colors = new List<string>
    {
        "Red", "Green", "Blue", "Black", "Purple", "Orange"
    };

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
            await UpdateExistingInvitationInDb();
        }

        return;
    }

    private void ResetForm()
    {
        CanUpdateExistingInvitation = false;
        CreateNewInstanceOfTypes();
    }

    private async Task SetAllInvitations()
    {
        var response = await MultiService.InvitationService.GetAllInvitations();

        if (response.IsFailed || !response.Value.Any())
        {
            ManyInvitations = new List<InvitationDto>();
            return;
        }


        ManyInvitations = response.Value
            .Where(x => x.EventStartAt >= DateTime.Today)
            .OrderBy(x => x.EventStartAt)
            .ToList();

        var passed = response.Value
            .Where(x => x.EventStartAt < DateTime.Today)
            .OrderByDescending(x => x.EventStartAt)
            .ToList();

        ManyInvitations.Add(new InvitationDto() { EventIntroduction = "SkipThisToShowWhichInvitationsHavePassed" });

        ManyInvitations.AddRange(passed);
    }

    #endregion

    #region SaveUpdateDeleteInvitation

    private async Task SaveInvitation()
    {
        var userInfo = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var userEmail = userInfo.User.Identity?.Name;
        Invitation.UpdatedByUser = userEmail ?? "Unknown. Caused in Server.Invitations.razor";

        if (CanUpdateExistingInvitation)
        {
            ShowSaveOptionsModal = true;
            return;
        }

        await SaveNewInvitationToDb();
    }

    private async Task SaveNewInvitationToDb()
    {
        var response = await MultiService.InvitationService.CreateInvitation(Invitation);

        if (response.IsFailed)
        {
            IsValidInvitation = false;
        }

        CloseSaveOptionsModal();
        CreateNewInstanceOfTypes();
    }

    private async Task UpdateExistingInvitationInDb()
    {
        var response = await MultiService.InvitationService.UpdateInvitation(Invitation, Invitation.Id);


        if (response.IsFailed)
        {
            CanUpdateExistingInvitation = false;
            CloseSaveOptionsModal();
            return;
        }

        CanUpdateExistingInvitation = true;
        CloseSaveOptionsModal();
        GoBackToEventPage();
        
    }

 

    private async Task DeleteInvitation()
    {
        var response = await MultiService.InvitationService.DeleteInvitation(InvitationToBeDeleted.Id);

        if (response.IsSuccess)
        {
            CloseConfirmDeletionModal();
            await SetAllInvitations();
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
        ShowInvitationPreview = true;
        if (inv is not null)
            Invitation = inv;
        CanUpdateExistingInvitation = true;
        MailTo = $"mailto:{Invitation.ContactEmail}?subject=Användarmöte";
    }

    private void ClosePreviewInvitation()
    {
        ShowInvitationPreview = false;
        MailTo = string.Empty;
    }

    private void SelectInvitationAndCloseModal(InvitationDto inv)
    {
        Invitation = inv;
        CanUpdateExistingInvitation = true;
        ShowInvitationsModal = false;
        CheckAllSectionsForIncorrectHyperlinkFormat();
    }

    private void OpenConfirmDeletionModal(InvitationDto inv)
    {
        ShowConfirmDeletionModal = true;
        InvitationToBeDeleted = inv;
    }

    private void CloseConfirmDeletionModal()
    {
        ShowConfirmDeletionModal = false;
        InvitationToBeDeleted = null;
    }

    private void CloseSaveOptionsModal()
    {
        ShowSaveOptionsModal = false;
       
    }

    private async Task OpenInvitationsModal()
    {
        ShowInvitationsModal = true;

        await SetAllInvitations();
    }

    private void CloseInvitationsModal()
    {
        ShowInvitationsModal = false;
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
}