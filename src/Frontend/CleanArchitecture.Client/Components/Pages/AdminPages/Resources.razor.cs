using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Enums;
using CleanArchitecture.Presentation.FrontendService;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;

namespace CleanArchitecture.Client.Components.Pages.AdminPages;

public partial class Resources : ComponentBase
{

    private bool ResourceExists => AllResources.Any();
    private bool ShowCreateDocumentModal { get; set; }
    private bool ShowUpdateDocumentModal { get; set; }

    private bool ToggleNameSorting { get; set; }
    private bool ToggleResourceNameSorting { get; set; }
    private List<DocumentDto> AllDocuments { get; set; } = new();

    private DocumentDto Document { get; set; } = new();




    [Inject] AuthenticationStateProvider AuthenticationStateProvider { get; set; }
    [Inject] private MultiService MultiService { get; set; }
    private List<ResourceDto> AllResources { get; set; } = new();
    private List<DocumentDto> Images { get; set; } = new();
    private List<DocumentDto> OtherDocuments { get; set; } = new();
    private ResourceDto Resource { get; set; } = new();
    private string ModalMessage { get; set; } = string.Empty;
    private bool ShowCreateResourceModal { get; set; }
    private bool IsLoadingResources { get; set; }
    private bool IsLoadingDocuments { get; set; }
    private bool ShowDocumentsModal { get; set; }
    private bool ShowEventsModal { get; set; }
    private bool ShowMessageModal { get; set; }
    private bool ShowUpdateResourceModal { get; set; }
    public bool ShowConfirmDeletionModal { get; set; }
    private bool ShowImagesInDocumentsModal { get; set; }
    private bool ShowOtherInDocumentsModal { get; set; }
    private bool ShowAllDocumentsInDocumentsModal { get; set; }


    protected override async Task OnInitializedAsync()
    {
        Document = new DocumentDto()
        {
            Resource = new ResourceDto()
        };
        await GetAllResourcesFromDbAsync();

    }

    #region Resources

    private async Task GetAllResourcesFromDbAsync()
    {
        IsLoadingResources = true;

        try
        {
            var resources = await MultiService.ResourceService.GetAllResources();

            if (resources.IsFailed)
            {
                IsLoadingResources = false;
                OpenMessageModal(resources.Errors.FirstOrDefault()?.Message ?? "Unknown error");
                return;
            }
            AllResources.Clear();
            AllResources.AddRange(resources.Value);
            StateHasChanged();

        }
        catch (Exception e)
        {
            IsLoadingResources = false;
            OpenMessageModal($"Could not get resources. \n{e.Message}");
        }

        AllResources = AllResources.OrderBy(x => x.ResourceName).ToList();
        IsLoadingResources = false;
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

    #region Create resource
    private async void OpenCreateResourceModal()
    {
        Resource = new ResourceDto();

        ShowCreateResourceModal = true;

        var auth = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var name = auth.User.Identity?.Name;
        Resource.CreatedBy = name ?? "unknown";

    }

    private void CloseCreateResourceModal()
    {
        ShowCreateResourceModal = false;

    }

    private async Task AddResourceToDbAsync()
    {
        try
        {
            var result = await MultiService.ResourceService.AddResourceAsync(Resource);

            if (result.IsFailed)
            {
                OpenMessageModal(result.Errors.FirstOrDefault()?.Message ?? "Unknown error");
            }

            AllResources.Clear();
            await GetAllResourcesFromDbAsync();

            ShowCreateResourceModal = false;

        }
        catch (Exception ex)
        {
            OpenMessageModal($"Could not save resource. {ex.Message}");
            return;
        }



    }
    #endregion

    #region Update resource
    private async Task OpenUpdateResourceModal(ResourceDto resource)
    {
        ShowUpdateResourceModal = true;

        Resource = (ResourceDto)resource.Clone();

        var auth = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var name = auth.User.Identity?.Name;
        Resource.CreatedBy = name ?? "unknown";
    }

    private void CloseUpdateResourceModal()
    {
        ShowUpdateResourceModal = false;
    }

    private async Task UpdateResourceInDbAsync()
    {
        try
        {
            var result = await MultiService.ResourceService.UpdateResourceAsync(Resource, Resource.ResourceId);

            if (result.IsFailed)
            {
                OpenMessageModal(result.Errors.FirstOrDefault()?.Message ?? "Unknown error");
            }

            AllResources.Clear();
            await GetAllResourcesFromDbAsync();

            AllResources = AllResources.OrderBy(x => x.ResourceName).ToList();

            ShowUpdateResourceModal = false;


        }
        catch (Exception ex)
        {
            OpenMessageModal($"Could not update resource. {ex.Message}");
            return;
        }



    }
    #endregion

    #region Delete
    private void OpenConfirmDeletionModal(ResourceDto resource)
    {
        Resource = resource;
        ShowConfirmDeletionModal = true;
    }

    private void CloseConfirmDeletionModal()
    {
        ShowConfirmDeletionModal = false;
        Resource = new();
    }

    private async Task DeleteResourceAsync()
    {
        try
        {
            var result = await MultiService.ResourceService.DeleteResourceAsync(Resource.ResourceId);

            if (result.IsFailed)
            {
                OpenMessageModal(result.Errors.FirstOrDefault()?.Message ?? "Unknown error");
            }

        }
        catch (Exception ex)
        {
            OpenMessageModal($"Could not delete resource. {ex.Message}");
            return;
        }

        AllResources.Remove(Resource);
        CloseConfirmDeletionModal();
    }
    #endregion

    private void OpenResourceDescriptionModal(ResourceDto resource)
    {
        OpenMessageModal(resource.ResourceDescription);
    }

    private void OpenResourceEventsModal(ResourceDto resource)
    {
        Resource = resource;
        ShowEventsModal = true;
    }

    private void CloseEventsModal()
    {
        ShowEventsModal = false;
    }

    #endregion



    #region Documents


    private async Task OpenResourceDocumentsModal(ResourceDto resource)
    {
        Resource = resource;
        ShowDocumentsModal = true;

        if (resource.DocumentCount > 0)
            await GetDocumentsByResourceId(resource.ResourceId);
    }

    private void CloseDocumentsModal()
    {
        ShowDocumentsModal = false;
    }

    private async Task GetDocumentsByResourceId(Guid id)
    {
        IsLoadingDocuments = true;


        try
        {
            var documents = await MultiService.DocumentService.GetDocumentsByResourceId(id);

            if (documents.IsFailed)
            {
                OpenMessageModal(documents.Errors.FirstOrDefault()?.Message ?? "Unknown error");
            }

            AllDocuments = documents.Value.OrderBy(x => x.FileName).ToList();
            ShowAllDocuments();

        }
        catch (Exception ex)
        {
            OpenMessageModal($"Could not get documents. {ex.Message}");
            IsLoadingDocuments = false;
            return;
        }

        IsLoadingDocuments = false;

    }

    #region Create new document

    private async Task OpenCreateDocumentModal()
    {
        var auth = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var name = auth.User.Identity?.Name;
        Document.CreatedBy = name ?? "";
        
        ShowCreateDocumentModal = true;

        if (AllResources.Any())
        {
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

        try
        {
            if (Document.File is null)
            {
                OpenMessageModal("Please pick a file to upload before saving document.");
                return;
            }

            var result = await MultiService.DocumentService.AddDocumentAsync(Document);

            if (result.IsFailed)
            {
                OpenMessageModal(result.Errors.FirstOrDefault()?.Message ?? "Unknown error");
                return;
            }

            Document = new DocumentDto()
            {
                Resource = new ResourceDto()
            };

            await GetAllResourcesFromDbAsync();
            StateHasChanged();
            ShowCreateDocumentModal = false;

        }
        catch (Exception ex)
        {
            OpenMessageModal($"Could not save document. {ex.Message}");
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

    #region Update

    private async Task OpenUpdateDocumentModal(DocumentDto document)
    {
        CloseDocumentsModal();

        await Task.Delay(1000);

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


        }
        catch (Exception ex)
        {
            OpenMessageModal($"Could not update document. {ex.Message}");
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

            await GetAllResourcesFromDbAsync();
            StateHasChanged();




        }
        catch (Exception e)
        {
            OpenMessageModal($"Could not delete document. \n{e.Message}");
            return;
        }
    }

    private void ShowImages()
    {
        ShowOtherInDocumentsModal = false;
        ShowAllDocumentsInDocumentsModal = false;
        ShowImagesInDocumentsModal = true;

        var images = AllDocuments.Where(d => d.FileType == FileType.Image).ToList();
        Images = images;

    }

    private void ShowOtherDocuments()
    {
        ShowImagesInDocumentsModal = false;
        ShowOtherInDocumentsModal = true;
        ShowAllDocumentsInDocumentsModal = false;

        var otherDocuments = AllDocuments.Where(d => d.FileType == FileType.Other).ToList();
        OtherDocuments = otherDocuments;
    }

    private void ShowAllDocuments()
    {
        ShowAllDocumentsInDocumentsModal = true;
        ShowOtherInDocumentsModal = false;
        ShowImagesInDocumentsModal = false;

    }

    #endregion

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



    private async Task NavigateToOpenResourceDocumentsModal(ResourceDto resource)
    {
        ShowConfirmDeletionModal = false;
        Resource = resource;
        ShowDocumentsModal = true;

        if (resource.DocumentCount > 0)
        {
            await GetDocumentsByResourceId(resource.ResourceId);
        }
    }
}