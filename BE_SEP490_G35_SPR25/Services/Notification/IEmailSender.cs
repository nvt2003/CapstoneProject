using System.Net.Mail;

namespace Planify_BackEnd.Services.Notification
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
        Task SendEmailWithAttachmentsAsync(string email, string subject, string message, List<Attachment> attachments);
    }
}
