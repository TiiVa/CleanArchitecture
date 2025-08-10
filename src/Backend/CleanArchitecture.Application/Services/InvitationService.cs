using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.DTOs.Converters;
using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Application.Interfaces.RepositoryInterfaces;
using CleanArchitecture.Application.Interfaces.ServiceInterfaces;
using FluentResults;

namespace CleanArchitecture.Application.Services;

public class InvitationService : IInvitationService
{
    private readonly IUnitOfWork _uow;
    private readonly ISerilogLogger _logger;

    public InvitationService(IUnitOfWork uow, ISerilogLogger logger)
    {
        _uow = uow;
        _logger = logger;
    }


    public async Task<Result> CreateInvitation(InvitationDto invitationDto)
    {

        try
        {
            if (invitationDto is null)
            {
                _logger.LogInformation("invitation is Null");
                return Result.Fail("Ofullständig inbjudan.");
            }

            var remadeInvitation = InvitationConvertTo.ConvertToInvitationForSave(invitationDto);

            var success = await _uow.CreateRepository<IInvitationRepository>().AddAsync(remadeInvitation);

            if (!success)
            {
                _logger.LogInformation("Could not Create Invitation");
                return Result.Fail("Kunde inte skapa inbjudan.");
            }
            await _uow.CompleteAsync();
            _logger.LogInformation("invitation created");
            return Result.Ok();
        }
        catch (Exception e)
        {
            _logger.LogError("Error", e);
            return Result.Fail(new Error("Ett oväntat fel har inträffat.").CausedBy(e));
        }

      
    }

    public async Task<Result<InvitationDto>> GetInvitationById(Guid id)
    {
        try
        {
            var invitation = await _uow.CreateRepository<IInvitationRepository>().GetByIdAsync(id);
            if (invitation is null)
            {
                _logger.LogInformation($"invitation with ID: {id} Not found");
                return Result.Fail($"Kunde inte hitta inbjudan med id {id}.");
            }

            var invitationToShow = invitation.ConvertToDto();
            _logger.LogInformation($"invitation with ID: {id} found");
            return Result.Ok(invitationToShow);
        }
        catch (Exception e)
        {
            _logger.LogError("Error", e);
            return Result.Fail(new Error("Ett oväntat fel har inträffat.").CausedBy(e));
        }


    }

    public async Task<Result<IEnumerable<InvitationDto>>> GetAllInvitations()
    {

        try
        {
            var listOfInvitations = await _uow.CreateRepository<IInvitationRepository>().GetAllAsync();

            if (!listOfInvitations.Any())
            {
                _logger.LogInformation("No Invitations found");
                return Result.Fail("Inga inbjudningar hittades.");
            }

            _logger.LogInformation("Invitations found");
            return Result.Ok(listOfInvitations.Select(i => i.ConvertToDto()));
        }
        catch (Exception e)
        {
            _logger.LogError("Error", e);
            return Result.Fail(new Error("Ett oväntat fel har inträffat").CausedBy(e));
        }

    }

    public async Task<Result<List<InvitationDto>>> GetInvitationsForEventsOpenForRegistration()
    {

        try
        {
            var invitationsList = await _uow.CreateRepository<IInvitationRepository>()
                .GetInvitationsForEventsOpenForRegistrationAsync();

            if (!invitationsList.Any())
            {
                _logger.LogInformation("No Invitations found");
                return Result.Fail("Inga inbjudningar hittades.");
            }

            List<InvitationDto> invitationsToShow = new();

            foreach (var invite in invitationsList)
            {
                var inv = invite.ConvertToDto();

                invitationsToShow.Add(inv);

            }
            _logger.LogInformation("Invitations Open for registration found");
            return Result.Ok(invitationsToShow);
        }
        catch (Exception e)
        {
            _logger.LogError("Error", e);
            return Result.Fail(new Error("Ett oväntat fel har inträffat.").CausedBy(e));
        }



    }

    public async Task<Result> UpdateInvitation(InvitationDto invitationDto, Guid id)
    {

        try
        {
            var remadeInvitation = InvitationConvertTo.ConvertToInvitation(invitationDto);

            var updated = await _uow.CreateRepository<IInvitationRepository>().UpdateAsync(remadeInvitation, id);

            if (!updated)
            {
                _logger.LogInformation("Invitation could not be Updated");
                return Result.Fail("Inbjudan kunde inte uppdateras.");
            }
            await _uow.CompleteAsync();
            _logger.LogInformation("Updated Invitation successfully");
            return Result.Ok();
        }
        catch (Exception e)
        {
            _logger.LogError("Error", e);
            return Result.Fail(new Error("Ett oväntat fel har inträffat."));
        }

        
    }

    public async Task<Result> DeleteInvitation(Guid id)
    {

        try
        {
            var deleteInvitation = await _uow.CreateRepository<IInvitationRepository>().RemoveAsync(id);

            if (!deleteInvitation)
            {
                _logger.LogInformation($"Could not Delete Invitation with ID: {id}");
                return Result.Fail($"Kunde inte ta bort inbjudan med id {id}");
            }
            await _uow.CompleteAsync();
            _logger.LogInformation("Invitation Deleted successfully.");
            return Result.Ok();
        }
        catch (Exception e)
        {
           _logger.LogError("Error",e);
            return Result.Fail(new Error("Ett oväntat fel har inträffat.").CausedBy(e));
        }
       
    }
}