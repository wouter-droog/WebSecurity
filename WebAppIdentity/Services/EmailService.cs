using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using WebAppIdentity.Settings;

namespace WebAppIdentity.Services;

public class EmailService : IEmailService
{
    private readonly SmtpSetting _smtpSettings;
    
    public EmailService(IOptions<SmtpSetting> options)
    {
        _smtpSettings = options.Value;
    }

    public async Task Send(string to, string subject, string body)
    {
        var email = new MailMessage(_smtpSettings.From, to)
        {
            Subject = subject,
            Body = body
        };
            
        // send email
        using (var emailClient = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port))
        {
            emailClient.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
            emailClient.EnableSsl = true;
            await emailClient.SendMailAsync(email);
        }
    }
}