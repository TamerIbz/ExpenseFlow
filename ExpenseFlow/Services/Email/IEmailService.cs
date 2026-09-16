using ExpenseFlow.Models;

namespace ExpenseFlow.Services;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string body);
    Task SendConfirmAccountEmailAsync(string toEmail,string confirmLink);
    Task SendResetPasswordEmailAsync(string toEmail,string resetLink);
}