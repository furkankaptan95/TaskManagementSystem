using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;

namespace EmailAPI;
public class SmtpConfiguration
{
    public string Server { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public bool EnableSsl { get; set; }
}
public class SmtpEmailService : IEmailService
{
    private readonly SmtpConfiguration _smtpConfiguration;
    public SmtpEmailService(IOptions<SmtpConfiguration> smtpConfiguration)
    {
        _smtpConfiguration = smtpConfiguration.Value;
    }
    public async Task<bool> SendEmailAsync(EmailRequest emailRequest)
    {
        try
        {
            var message = new MailMessage
            {
                From = new MailAddress(_smtpConfiguration.Username),
                Subject = emailRequest.Subject,
                Body = emailRequest.Body,
                IsBodyHtml = true,
            };

            message.To.Add(emailRequest.To);

            var smtpClient = new SmtpClient(_smtpConfiguration.Server)
            {
                Port = _smtpConfiguration.Port,
                Credentials = new NetworkCredential(_smtpConfiguration.Username, _smtpConfiguration.Password),
                EnableSsl = _smtpConfiguration.EnableSsl
            };

            await smtpClient.SendMailAsync(message);
            return true;
        }

        catch (Exception)
        {
            return false;
        }
    }
}
