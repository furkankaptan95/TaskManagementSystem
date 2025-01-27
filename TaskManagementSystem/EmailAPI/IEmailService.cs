namespace EmailAPI;
public interface IEmailService
{
    Task<bool> SendEmailAsync(EmailRequest emailRequest);
}
