using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Application.Interfaces.ServiceInterfaces;
using CleanArchitecture.Infrastructure.ExternalServices.Email;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace CleanArchitecture.Infrastructure.ExternalServices;

public class EmailService : IEmailService
{
    private readonly MailAppSettings _appSettings;
    private readonly ISerilogLogger _logger;


    public EmailService(IOptions<MailAppSettings> appSettings, ISerilogLogger logger)
    {
        
        _logger = logger;
        _appSettings = appSettings.Value;
    }

    public async Task Send(string to, string subject, string html)
    {

        try
        {

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_appSettings.EmailFrom));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = html };

            using var smtp = new SmtpClient();
            _logger.LogInformation("SMTP client created.");

            await smtp.ConnectAsync(_appSettings.SmtpHost, _appSettings.SmtpPort, SecureSocketOptions.StartTls);
            _logger.LogInformation("Connection established to SMTP server.");

            await smtp.AuthenticateAsync(_appSettings.SmtpUser, _appSettings.SmtpPass);
            _logger.LogInformation("Authenticated to SMTP server.");

            await smtp.SendAsync(email);
            _logger.LogInformation($"Email sent to {to}.");

            await smtp.DisconnectAsync(true);
            _logger.LogInformation("Disconnected from SMTP server.");
        }
        catch (Exception e)
        {
            _logger.LogError("Error",e);
            throw;
        }

       
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        try
        {
            await Send(email, subject, htmlMessage);
            _logger.LogInformation("Building send message method");
        }
        catch (Exception e)
        {
            _logger.LogError("Error",e);
            throw;
        }
      
    }
   
}