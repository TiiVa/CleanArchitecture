using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.DTOs.Converters;
using CleanArchitecture.Application.DTOs.UserDtos;
using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Application.Interfaces.RepositoryInterfaces;
using CleanArchitecture.Application.Interfaces.ServiceInterfaces;
using CleanArchitecture.Domain.Enums;
using FluentResults;

namespace CleanArchitecture.Application.Services;

public class DocumentService : IDocumentService
{
    private readonly IUnitOfWork _uow;
    private readonly ISerilogLogger _logger;


    public DocumentService(IUnitOfWork uow, ISerilogLogger logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<DocumentDto>>> GetAllDocuments()
    {

        try
        {
            var allDocuments = await _uow.CreateRepository<IDocumentRepository>().GetAllAsync();

            if (!allDocuments.Any())
            {
                _logger.LogInformation("No Documents found.");
                return Result.Fail("Inga dokument hittades");
            }
            _logger.LogInformation("Documents found.");
            return Result.Ok(allDocuments.Select(d => d.ConvertToDto()));
        }
        catch (Exception e)
        {
            _logger.LogError("Error", e);
            return Result.Fail(new Error("Ett oväntat fel inträffade").CausedBy(e));
        }


    }
    
    public async Task<Result> AddDocumentAsync(DocumentDto? document)
    {

        try
        {
            if (document is null)
            {
                _logger.LogInformation("The document has invalid values");
                return Result.Fail("Dokumentet är felaktigt");
            }

            var resource = await _uow.CreateRepository<IResourceRepository>().GetByIdAsync(document.Resource.ResourceId);
            var user = await _uow.CreateRepository<IApplicationUserRepository>().GetAllInfoByEmailAsync(document.CreatedBy);



            var documentModel = document.ConvertToModelWithoutObjects();
            documentModel.Resource = resource;
            documentModel.ApplicationUser = user;

            if (!string.IsNullOrWhiteSpace(document.NewFileName))
            {
                var extension = Path.GetExtension(document.FileName);
                documentModel.Name = document.NewFileName + extension;
                _logger.LogInformation("Document path created.");
            }

            var success = await _uow.CreateRepository<IDocumentRepository>().AddAsync(documentModel);

            if (!success)
            {
                _logger.LogInformation("Failed to add new document.");

                return Result.Fail("Kunde inte spara ändringar");
            }
            await _uow.CompleteAsync();
            _logger.LogInformation("Successfully added a Document.");
            return Result.Ok();

        }
        catch (Exception e)
        {
            _logger.LogError("Error", e);
            return Result.Fail(new Error("Ett oväntat fel inträffade").CausedBy(e));
        }
      
    }

    public async Task<Result> UpdateDocumentAsync(DocumentDto document, Guid id)
    {


        try
        {
            var resource = await _uow.CreateRepository<IResourceRepository>().GetByIdAsync(document.ResourceId);
            var user = await _uow.CreateRepository<IApplicationUserRepository>().GetAllInfoByEmailAsync(document.CreatedBy);

            if (resource is null)
            {
                _logger.LogInformation("Resource not found in Document");
                return Result.Fail("Kunde inte hitta mappen");
            }

            if (user is null)
            {
                _logger.LogInformation("User not found in Document");
                return Result.Fail("Kunde inte hitta användaren");
            }


            var documentModel = document.ConvertToModelWithoutObjects();
            documentModel.Resource = resource;
            documentModel.ApplicationUser = user;

            if (!string.IsNullOrWhiteSpace(document.NewFileName))
            {
                var extension = Path.GetExtension(document.FileName);
                documentModel.Name = document.NewFileName + extension;
            }

            var success = await _uow.CreateRepository<IDocumentRepository>().UpdateAsync(documentModel, id);

            if (!success)
            {
                _logger.LogInformation("Failed to update document.");
                return Result.Fail("Kunde inte uppdatera dokumentet");
            }
            await _uow.CompleteAsync();
            _logger.LogInformation("Successfully updated document.");

            return Result.Ok();

        }
        catch (Exception e)
        {
            _logger.LogError("Error", e);
            return Result.Fail(new Error("Ett oväntat fel inträffade").CausedBy(e));
        }
      

    }
    
    public async Task<Result<IEnumerable<DocumentDto>>> GetDocumentsByResourceId(Guid resourceId)
    {

        try
        {
            var documentsByResourceId =
                await _uow.CreateRepository<IDocumentRepository>().GetDocumentsByResourceId(resourceId);

            if (!documentsByResourceId.Any())
            {
                _logger.LogInformation("No documents for this resource found");
                return Result.Fail("Inga dokument för denna resurs hittade");
            }
            _logger.LogInformation("Documents for this resource found");
            return Result.Ok(documentsByResourceId.Select(d => d.ConvertToDto()));
        }
        catch (Exception e)
        {
            _logger.LogError("Error",e);
            return Result.Fail(new Error("Ett oväntat fel inträffade").CausedBy(e));
        }
   
    }

    public async Task<FileDataDto?> GetFileData(Guid documentId)
    {
        try
        {
            var document = await _uow.CreateRepository<IDocumentRepository>().GetByIdAsync(documentId);

            if (document is null)
            {
                _logger.LogInformation($"Document with ID: {documentId} not found.");
            }

            var fileDataDto = new FileDataDto(document.Name, document.File ?? Array.Empty<byte>(), document.Type);

            return fileDataDto.Data == Array.Empty<byte>() ? null : fileDataDto;
        }
        catch (Exception e)
        {
           _logger.LogError("Error", e);
           throw;
        }
    }

    public async Task<List<FileDataDto>> GetImagesForUserByEventId(Guid eventId)
    {
        try
        {
            var documents = await _uow.CreateRepository<IDocumentRepository>().GetImagesForUserByEventId(eventId);

            if (!documents.Any())
            {
                _logger.LogInformation("No Images in this Event found.");
            }
            
            var documentsOfTypeImage = documents.Where(d => d.Type == FileType.Image);

            var images = documentsOfTypeImage.Select(document => new FileDataDto(
                    document.Name,
                    document.File ?? Array.Empty<byte>(),
                    document.Type))
                .ToList();


            images = images
                .Where(x => x.Data != Array.Empty<byte>())
                .ToList();
            _logger.LogInformation("Images for this Event found.");
            return images;
        }
        catch (Exception e)
        {
            _logger.LogError("error",e);
            throw;
        }
      

    }
    public async Task<Result> DeleteDocumentAsync(Guid id)
    {
        try
        {
            var success = await _uow.CreateRepository<IDocumentRepository>().RemoveAsync(id);

            if (!success)
            {
                _logger.LogInformation($"Could not Deleted document with ID:{id}");
                return Result.Fail("Failed to delete document ");
            }
            await _uow.CompleteAsync();
            _logger.LogInformation("Successfully Deleted document.");
            return Result.Ok();

        }
        catch (Exception e)
        {
            _logger.LogError("Error",e);
            return Result.Fail(new Error("Ett oväntat fel inträffade").CausedBy(e));
        }
 

    }
}