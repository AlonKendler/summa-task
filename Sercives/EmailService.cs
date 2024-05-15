using System.IO;
using Resend;

namespace summa_task.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string to, string subject, string formattedText, string extractedText, string from);
    }

    public class ResendEmailService : IEmailService
    {
        private readonly IResend _resend;
        private readonly string _emailTemplateFilePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "ReceiptEmail.html");

        public ResendEmailService(IResend resend)
        {
            _resend = resend;
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string formattedText, string extractedText, string from)
        {
            try
            {
                // Read the email template
                var emailTemplate = await File.ReadAllTextAsync(_emailTemplateFilePath);

                // Replace placeholders with actual values
                emailTemplate = emailTemplate.Replace("@Model.FormattedText", formattedText);
                emailTemplate = emailTemplate.Replace("@Model.ExtractedText", extractedText);

                var msg = new EmailMessage
                {
                    From = from,
                    To = { to },
                    Subject = subject,
                    HtmlBody = emailTemplate
                };

                await _resend.EmailSendAsync(msg);
                return true; // Indicate successful email sending
            }
            catch (Exception ex)
            {
                // Log the email sending error for debugging
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false; // Indicate email sending failure
            }
        }
    }
}