using AuthApi.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AuthApi.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendPasswordResetEmailAsync(string email, string resetToken)
    {
        // Pour cet exemple, on simule l'envoi d'email
        // En production, utilisez SendGrid, MailKit, ou un autre service d'email
        
        _logger.LogInformation($"Password reset email sent to {email}. Token: {resetToken}");
        
        // TODO: Implémenter l'envoi réel d'email
        // Exemple avec SendGrid:
        // var client = new SendGridClient(_configuration["SendGrid:ApiKey"]);
        // var message = new SendGridMessage();
        // message.SetFrom(new EmailAddress(_configuration["Email:From"], "AuthApi"));
        // message.AddTo(new EmailAddress(email));
        // message.SetSubject("Reset your password");
        // message.SetHtmlContent($"<p>Your reset token: {resetToken}</p>");
        // await client.SendEmailAsync(message);
        
        await Task.CompletedTask;
    }
}