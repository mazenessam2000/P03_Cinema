using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace P03_Cinema.Utility;

public class EmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var client = new SmtpClient("smtp.gmail.com", 587)
        {
            EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential("yawala0001@gmail.com", "dhvg uemq yibi mdci")
        };

        return client.SendMailAsync(
            new MailMessage(from: "yawala0001@gmail.com",
            to: email,
            subject,
            htmlMessage)
            {
                IsBodyHtml = true
            });
    }
}