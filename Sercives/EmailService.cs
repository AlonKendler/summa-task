using Resend;

namespace summa_task.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body, string from);
    }

    public class ResendEmailService : IEmailService
    {
        private readonly IResend _resend;

        public ResendEmailService(IResend resend)
        {
            _resend = resend;
        }

        public async Task SendEmailAsync(string to, string subject, string body, string from)
        {
            var msg = new EmailMessage
            {
                From = from,
                To = { to },
                Subject = subject,
                HtmlBody = body
            };

            await _resend.EmailSendAsync(msg);
        }
    }
}