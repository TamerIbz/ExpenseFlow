using System.Net;
using System.Net.Mail;
using Azure.Core;
using ExpenseFlow.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;

namespace ExpenseFlow.Services;

public class EmailService : IEmailService
{
    public EmailService(IOptions<EmailSettings> options)
    {
        // _configuration = configuration;
        _emailSettings = options.Value;
    }

    // private readonly IConfiguration _configuration;
    private readonly EmailSettings _emailSettings;
    
    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var message = new MailMessage(_emailSettings.From!, toEmail, subject, body);
        message.IsBodyHtml = true;
        
        using var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port)
        {
            Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
            EnableSsl = true
        };

        await client.SendMailAsync(message);
    }

    public async Task SendConfirmAccountEmailAsync(string toEmail, string confirmLink)
    {
        var subject = "Confirm Account";
        // var body = $"Please confirm your account by clicking here: <a href='{confirmLink}'>Confirm Account</a>";
        var body = $"" +
                   $"Please confirm your account by clicking here: " +
                   $"<a href='{confirmLink}'>Confirm Account</a>" +
                   $"<br/>This link will expire in 2 hours for security reasons.\n\nIf you did not register an account, you can safely ignore this email.";
        await SendEmailAsync(toEmail, subject, body);
    }

    public async Task SendResetPasswordEmailAsync(string toEmail, string resetLink)
    {
        const string subject = "Reset Password";
        var body = $"" +
                   $"Please reset your password by clicking here: " +
                   $"<a href='{resetLink}'>Reset Password</a>" +
                   $"<br/>This link will expire in 2 hours for security reasons.\n\nIf you did not request a password reset, you can safely ignore this email.";
        await SendEmailAsync(toEmail, subject, body);
    }
}