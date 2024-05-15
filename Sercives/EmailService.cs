using Resend;

namespace summa_task.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string to, string subject, string body, string from);
    }

    public class ResendEmailService : IEmailService
    {
        private readonly IResend _resend;

        public ResendEmailService(IResend resend)
        {
            _resend = resend;
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string body, string from)
        {
            var msg = new EmailMessage
            {
                From = from,
                To = { to },
                Subject = subject,
                HtmlBody = body
            };

            try
            {
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
