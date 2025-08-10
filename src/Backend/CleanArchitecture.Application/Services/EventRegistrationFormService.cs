using System.Text;
using System.Text.Encodings.Web;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.DTOs.Converters;
using CleanArchitecture.Application.DTOs.UserDtos;
using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Application.Interfaces.RepositoryInterfaces;
using CleanArchitecture.Application.Interfaces.ServiceInterfaces;
using CleanArchitecture.Domain.Entities;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace CleanArchitecture.Application.Services;

public class EventRegistrationFormService : IEventRegistrationFormService
{
    private readonly IUnitOfWork _uow;
    private readonly ISerilogLogger _logger;
    private readonly IPasswordGeneratorService _passwordService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;

    public EventRegistrationFormService(IUnitOfWork uow, IPasswordGeneratorService passwordService, UserManager<ApplicationUser> userManager, ISerilogLogger logger,IEmailService emailService)
    {
        _uow = uow;
        _passwordService = passwordService;
        _userManager = userManager;
        _logger = logger;
        _emailService = emailService;
    }

    public async Task<Result<IEnumerable<EventRegistrationFormDto>>> GetAllAsync()
    {
        try
        {
            var allForms = await _uow.CreateRepository<IEventRegistrationFormRepository>().GetAllAsync();

            if (!allForms.Any())
            {
                _logger.LogInformation("No UserRegistrationForm found");
                return Result.Fail("Inga anmälningar hittades.");
            }
            var allFormsDtoList = allForms.Select(u => u.ConvertToDto());
            _logger.LogInformation("UserRegistrationForms found");
            return Result.Ok(allFormsDtoList);

        }
        catch (Exception e)
        {
            _logger.LogError("Error", e);
            return Result.Fail(new Error("Ett oväntat fel har inträffat.").CausedBy(e));
        }

    }

    public async Task<Result> AddAsync(EventRegistrationSparseDto? userInput)
    {
        try
        {
            if (userInput is null)
            {
                _logger.LogInformation("Registration form are empty.");
                return Result.Fail("Formuläret är ofullständigt.");
            }

            var dto = userInput.ConvertToDto(); // Convert UserMeetingRegistrationSparseDto to UserMeetingRegistrationFormDto

            var form = dto.ConvertToModel(); // Convert UserMeetingRegistrationFormDto to UserMeetingRegistrationForm (entity)

            var userIsAlreadyRegistered = await _uow.CreateRepository<IEventRegistrationFormRepository>()
                .UserIsAlreadyRegisteredToEvent(form); // Check if user is already registered

            if (userIsAlreadyRegistered)
            {
                _logger.LogInformation("Failed, user already registered to this form.");
                return Result.Fail($"Användare {userInput.Email} är redan anmäld till evenemanget." +
                                         $"\n\nVid frågor kontakta oss " +
                                         $"\n{userInput.Event.ContactEmail}" +
                                         $"\n{userInput.Event.ContactInfo}");
            }

            var eventForForm = await _uow.CreateRepository<IEventRepository>().GetById(form.Event.Id); // Check that the event exists
            if (eventForForm is null)
            {
                _logger.LogInformation("Failed, Event doesn't exist.");

                return Result.Fail("Webbservern stötte på ett oväntat fel som hindrade den från att fullfölja begäran. Vänligen försök igen.");
            }

            form.Event = eventForForm; // Set the event to the registration

            await _uow.CreateRepository<IEventRegistrationFormRepository>().AddAsync(form); // Send the registration to repository to be added to database

            var password = _passwordService.Generate(); // Generate a new password for user and send it in below method to confirm participation

            await SendWelcomeMessageWithAccountConfirmation(form, password);

            await _uow.CompleteAsync();
            _logger.LogInformation("Form created and user registered.");

            return Result.Ok();
        }
        catch (Exception e)
        {
            _logger.LogError("Error", e);
            return Result.Fail(new Error("Ett oväntat fel har inträffat.").CausedBy(e));
        }

    }
    
    public async Task<Result> UpdateFormAsync(EventRegistrationFormDto form, Guid id)
    {

        try
        {
            var userMeetingRegistrationFormDto = form.ConvertToModel();

            var success = await _uow.CreateRepository<IEventRegistrationFormRepository>()
                .UpdateAsync(userMeetingRegistrationFormDto, id);

            if (!success)
            {
                _logger.LogInformation("Could not updated Form for registration");
                return Result.Fail("Kunde inte uppdatera anmälan.");
            }
            await _uow.CompleteAsync();
            _logger.LogInformation("Could update Form for registration");
            return Result.Ok();

        }
        catch (Exception e)
        {
            _logger.LogError("Error", e);
            return Result.Fail(new Error("Ett oväntat fel har inträffat.").CausedBy(e));
        }

    }

    public async Task<Result> DeleteFormAsync(Guid id)
    {
        try
        {
            var success = await _uow.CreateRepository<IEventRegistrationFormRepository>().RemoveAsync(id);

            if (!success)
            {
                _logger.LogInformation($"Could not Delete form with ID: {id}");
                return Result.Fail($"Kunde inte ta bort anmälan med id {id}");
            }
            await _uow.CompleteAsync();
            _logger.LogInformation("Successfully Deleted Form.");
            return Result.Ok();

        }
        catch (Exception e)
        {
            _logger.LogError("Error", e);
            return Result.Fail(new Error("Ett oväntat fel har inträffat.").CausedBy(e));
        }
    }
    
    public async Task<Result<List<UserRegistrationInfoDto>>> GetRegistrationInfoByUserIdAsync(Guid id)
    {

        try
        {
            var forms = await _uow.CreateRepository<IEventRegistrationFormRepository>()
                .GetRegistrationInfoByUserIdAsync(id);

            if (forms == null)
            {
                _logger.LogInformation($"Could not find form with ID: {id}");
                return Result.Fail($"Kunde inte hitta anmälan med id {id}");
            }
            var registrationInfoDtoList = forms.Select(x => new UserRegistrationInfoDto(
                    $"{x.FirstName} {x.LastName}",
                    x.Email,
                    x.Company,
                    x.PhoneNumber,
                    x.Accommodation,
                    x.AccommodationWith,
                    x.InvoiceReference,
                    x.Allergies,
                    x.UserInformation,
                    x.Event.Invitation != null ? x.Event.Invitation.ContactInfo : "Anna Anderson, +46 70 11 22",
                    x.Event.Invitation != null ? x.Event.Invitation.ContactEmail : "anna@eventsphere.com",
                    x.Event.Name,
                    x.Event.EventDate))
                .ToList();

            _logger.LogInformation($"Form with ID: {id} found.");
            return Result.Ok(registrationInfoDtoList);
        }
        catch (Exception e)
        {
            _logger.LogError("Error", e);
            return Result.Fail(new Error("Ett oväntat fel har inträffat.").CausedBy(e));
        }
    }

    public async Task<Result> ConfirmParticipationInEvent(string userId, string eventId)
    {
        try
        {
            var success = await _uow.CreateRepository<IEventRegistrationFormRepository>()
                .ConfirmParticipationInEvent(userId, eventId);

            if (!success)
            {
                _logger.LogInformation("Confirmation to participant didn't go thru.");
                return Result.Fail("Det gick inte att bekräfta deltagandet i eventet.");
            }
            await _uow.CompleteAsync();
            _logger.LogInformation("Confirmation to participant went thru successfully.");

            return Result.Ok();

        }
        catch (Exception e)
        {
            _logger.LogError("Error", e);
            return Result.Fail(new Error("Ett oväntat fel har inträffat.").CausedBy(e));
        }
    }

    public async Task<Result> SendWelcomeMessageWithAccountConfirmation(EventRegistrationForm user, string password)
    {

        try
        {
            var existingUserAccount = await _uow.CreateRepository<IApplicationUserRepository>().GetAllInfoByEmailAsync(user.Email);

            if (existingUserAccount.Email is null)
            {
                _logger.LogInformation($"No matching Email in Db, created a new account with that Email and password.");

                var newAccount = await CreateDefaultAccountAsync(user.Email, password);

                if (newAccount.IsFailed)
                {
                    _logger.LogInformation($"Failed to Create Account.");
                    return Result.Fail("Kunde inte skapa nytt konto.");
                }
                
            }

            var message = await CreateWelcomeMessageWithAccountConfirmationLinkAsync(user.Email, password, user.Event.Id); // TODO: Denna aktiveras i ett senare skede

            

            await _emailService.SendEmailAsync(user.Email, "Välkommen från oss på EventSphere", message.Value); // TODO: Denna aktiveras i ett senare skede

            return Result.Ok();
        }
        catch (Exception e)
        {
            _logger.LogError("Error", e);
            return Result.Fail(new Error("Ett oväntat fel har inträffat.").CausedBy(e));
        }

    }

    public async Task<Result> CreateDefaultAccountAsync(string email, string password)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                _logger.LogInformation("Failed to create user.");
                return Result.Fail("Ogiltig e-mail.");
            }

            var applicationUser = new ApplicationUser { UserName = email, Email = email };
            var result = await _userManager.CreateAsync(applicationUser, password);

            if (!result.Succeeded)
            {
                _logger.LogInformation("Failed to create user.");
                return Result.Fail("Kunde inte skapa ny användare.");
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                _logger.LogInformation("User created but not found.");
                return Result.Fail("Användare saknas.");
            }

            var addRoleResult = await _userManager.AddToRoleAsync(user, "User");
            _logger.LogInformation("User created.");

            if (!addRoleResult.Succeeded)
            {
                _logger.LogInformation($"Could not add role to user {user.Email}");
                return Result.Fail("Kunde inte sätta roll på användaren.");
            }

            _logger.LogInformation("Role added.");
            return Result.Ok();
        }
        catch (Exception e)
        {
            _logger.LogError("Error", e);
            return Result.Fail(new Error("Ett oväntat fel har inträffat.").CausedBy(e));
        }

    }

    public async Task<Result<string>> CreateWelcomeMessageWithAccountConfirmationLinkAsync(string email, string password, Guid eventId) //TODO: Testa när EmailService är live
    {
        
        try
        {
            //Uri returnUrl = new Uri($@"/authentication/login");

            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                return Result.Fail($"Användare med email {email} kunde inte hittas.");
            }
            _logger.LogInformation("User found for email creation.");

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            _logger.LogInformation("Email confirmation token generated.");


            var uriBuilder = new UriBuilder("https://")
            {
                //Scheme = "https",
                Path = $@"Identity/Account/ConfirmEmail",
                Query = $@"userId={user.Id}&code={code}&returnUrl=%2Fauthentication%2Flogin&emailConfirmed={user.EmailConfirmed}&eventId={eventId}"
            };
            _logger.LogInformation("URI to confirm account built.");

            if (user.EmailConfirmed)
            {
                return Result.Ok($"<h2>Välkommen till EventSphere användarmöte!</h2> " +
                                 $"<p>Du är nu anmäld till årets användarmöte, vänligen <a href='{HtmlEncoder.Default.Encode(uriBuilder.Uri.ToString())}'>bekräfta din anmälan.</a></p>" +
                                 $"<p>Bästa hälsningar, <br>EventSphere-teamet</p>");
            }

            return Result.Ok($"<h2>Välkommen till EventSphere användarmöte!</h2> " +
                             $"<p>Du är nu anmäld till årets användarmöte, vänligen <a href='{HtmlEncoder.Default.Encode(uriBuilder.Uri.ToString())}'>bekräfta din anmälan.</a></p>" +
                             $"<p>Efter bekräftelsen har du möjlighet att logga in på vår hemsida. Ditt konto har användarnamn: {email} och lösenord: {password}</p>" +
                             $"<p>På hemsidan kan ni se er anmälan och våra tidigare evenemang tillsammans med dess presentationer och bilder.</p>" +
                             $"<p>Bästa hälsningar, <br>EventSphere-teamet</p>");


        }
        catch (Exception e)
        {
            _logger.LogError("Error", e);
            return Result.Fail(new Error("Ett oväntat fel har inträffat.").CausedBy(e));
        }

    }

}