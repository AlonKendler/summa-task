using System.Net.Mail;

public class EmailService
{
    public void SendEmailWithOcrOutput(string emailAddress, string ocrOutput)
    {
        // replace with sender email
        var smtpClient = new SmtpClient("smtp.example.com");
        var mailMessage = new MailMessage
        {
            From = new MailAddress("sender@example.com"),
            Subject = "Receipt OCR Output",
            Body = ocrOutput
        };
        mailMessage.To.Add(emailAddress);
        smtpClient.Send(mailMessage);
    }
}