using Microsoft.Extensions.Logging;
namespace SakilaApp.Services;

public interface IEmailSender
{
    Task SendEmailAsync(string to, string subject, string body);
}
