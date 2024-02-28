using HoneyWebPlatform.Services.Data.Interfaces;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using HoneyWebPlatform.Services.Data.Models;


public class EmailSender : IEmailSender
{
    private readonly EmailSettings _emailSettings;
    

    public EmailSender(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }

    public async Task SendEmailAsync(string email, string subject, string message, string number)
    {
        var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
        {
            EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword)
        };

        await client.SendMailAsync(
            new MailMessage(from: _emailSettings.SmtpUsername,
                to: email, 
                subject, 
                message
            ));
    }
}