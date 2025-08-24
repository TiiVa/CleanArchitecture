using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Presentation.FrontendService;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;

namespace CleanArchitecture.Client.Components.Pages.AdminPages;

public partial class Documents : ComponentBase
{
   
    [Inject] private MultiService MultiService { get; set; }
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }
    private bool ShowMessageModal { get; set; }
    private bool ResourceExists => AllResources.Any();
    private bool ShowCreateDocumentModal { get; set; }
    private bool ShowUpdateDocumentModal { get; set; }
    private bool IsLoadingDocuments { get; set; }
    private bool IsLoadingResources { get; set; }
    private bool ToggleNameSorting { get; set; }
    private bool ToggleResourceNameSorting { get; set; }
    private List<DocumentDto> AllDocuments { get; set; } = new();
    private List<ResourceDto> AllResources { get; set; } = new();
    private DocumentDto Document { get; set; } = new();
    private string ModalMessage { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        Document = new DocumentDto()
        {
            Resource = new ResourceDto()
        };
        await GetAllDocumentsFromDbAsync();
    }

    private async Task GetAllDocumentsFromDbAsync()
    {
        IsLoadingDocuments = true;
        try
        {
            var documents = await MultiService.DocumentService.GetAllDocuments();

            if (documents.IsFailed)
            {
                IsLoadingDocuments = false;
                OpenMessageModal(documents.Errors.FirstOrDefault()?.Message ?? "Unknown error");
                return;
            }

            AllDocuments = documents.Value.OrderBy(x => x.FileName).ToList();
            IsLoadingDocuments = false;
        }
        catch (Exception ex)
        {
            IsLoadingDocuments = false;
            OpenMessageModal($"Could not get documents. {ex.Message}");
        }

    }

    private async Task GetAllResourcesFromDbAsync()
    {
        IsLoadingResources = true;
        AllResources = new List<ResourceDto>();

        try
        {
            var resources = await MultiService.ResourceService.GetAllResources();

            if (resources.IsFailed)
            {
                IsLoadingResources = false;
                OpenMessageModal(resources.Errors.FirstOrDefault()?.Message ?? "Unknown error");
                return;
            }

            AllResources.AddRange(resources.Value);
            Document.Resource = AllResources[0];
            IsLoadingResources = false;
        }
        catch (Exception ex)
        {
            IsLoadingResources = false;
            OpenMessageModal($"Could not get resources. \n{ex.Message}");
        }
    }

    #region MessageModal

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

    #region Create new

    private async Task OpenCreateDocumentModal()
    {
        Document.FileName = string.Empty;
        Document.File = null;
        Document.Description = string.Empty;

        var auth = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var name = auth.User.Identity?.Name;
        Document.CreatedBy = name ?? "";
        ShowCreateDocumentModal = true;

        if (AllResources.Any())
        {
            //TODO kolla vad detta innebär
            //Document.Resource = AllResources[0];
            return;
        }

        await GetAllResourcesFromDbAsync();
    }

    private void CloseCreateDocumentModal()
    {
        ShowCreateDocumentModal = false;
    }

    private async Task AddDocumentToDbAsync()
    {
        if (Document.File is null)
        {
            OpenMessageModal("Please pick a file to upload before saving document.");
            return;
        }

        try
        {
            var result = await MultiService.DocumentService.AddDocumentAsync(Document);

            if (result.IsFailed)
            {
                OpenMessageModal(result.Errors.FirstOrDefault()?.Message ?? "Unknown error");
                return;
            }

            AllDocuments.Add(Document);

            AllDocuments = AllDocuments.OrderBy(x => x.FileName).ToList();

            ShowCreateDocumentModal = false;

            await OnInitializedAsync();
        }
        catch (Exception ex)
        {
            OpenMessageModal($"Could not save document. {ex.Message}");
            return;
        }

    }

    #endregion

    private async Task DeleteDocumentAsync(DocumentDto document)
    {
        try
        {
            var getDocument = await MultiService.DocumentService.DeleteDocumentAsync(document.DocumentId);

            if (getDocument.IsFailed)
            {
                OpenMessageModal(getDocument.Errors.FirstOrDefault()?.Message ?? "Unknown error");
                return;
            }

            AllDocuments.Remove(document);

        }
        catch (Exception e)
        {
            OpenMessageModal($"Could not delete document. \n{e.Message}");
            return;
        }
    }

    #region Update

    private async Task OpenUpdateDocumentModal(DocumentDto document)
    {
        ShowUpdateDocumentModal = true;

        Document = (DocumentDto)document.Clone();

        if (!AllResources.Any())
        {
            await GetAllResourcesFromDbAsync();
        }

        Document.Resource = document.Resource;
    }

    private void CloseUpdateModal()
    {
        ShowUpdateDocumentModal = false;
    }

    private async Task UpdateDocumentInDbAsync()
    {
        var auth = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        Document.CreatedBy = auth.User.Identity?.Name ?? "Unknown";

        try
        {
            var result = await MultiService.DocumentService.UpdateDocumentAsync(Document, Document.DocumentId);

            if (result.IsFailed)
            {
                OpenMessageModal(result.Errors.FirstOrDefault()?.Message ?? "Unknown error");
                return;
            }

            CloseUpdateModal();

            await OnInitializedAsync();

        }
        catch (Exception ex)
        {
            OpenMessageModal($"Could not update document. {ex.Message}");
            return;
        }


    }

    #endregion

    private async Task GetSelectedFileAsync(InputFileChangeEventArgs inputFileChangeEventArgs)
    {
        try
        {
            var file = inputFileChangeEventArgs.File;
            await using var stream = file.OpenReadStream(10000000);
            await using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            Document.File = memoryStream.ToArray();
            Document.FileName = file.Name;
            stream.Close();
        }
        catch (Exception ex)
        {
            OpenMessageModal($"Could not select document. {ex.Message}");
        }
    }

    private async Task DownloadDocumentAsync(DocumentDto doc)
    {
        if (doc.File is null)
        {
            OpenMessageModal("File data not found.");
            return;
        }

        await Js.DownloadFile(doc.FileName, doc.File);
    }

    #region SortBy
    private void SortByName()
    {
        AllDocuments = ToggleNameSorting
            ? AllDocuments.OrderBy(x => x.FileName).ToList()
            : AllDocuments.OrderByDescending(x => x.FileName).ToList();

        ToggleNameSorting = !ToggleNameSorting;
    }

    private void SortByResourceName()
    {
        AllDocuments = ToggleResourceNameSorting
            ? AllDocuments.OrderBy(x => x.Resource.ResourceName).ToList()
            : AllDocuments.OrderByDescending(x => x.Resource.ResourceName).ToList();

        ToggleResourceNameSorting = !ToggleResourceNameSorting;
    }
    #endregion
}