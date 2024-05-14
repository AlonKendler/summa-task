using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Google.Cloud.Vision.V1;
using Resend;

namespace summa_task.Pages
{

    public class IndexModel : PageModel
    {

        private readonly IResend _resend;
        private readonly EmailService _emailService;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(EmailService emailService, ILogger<IndexModel> logger, IResend resend)
        {
            _emailService = emailService;
            _logger = logger;
            _resend = resend;
        }


        public async Task<IActionResult> OnPostAsync(IFormFile receiptImage, string emailAddress)
        {
            try
            {
                _logger.LogInformation("Received receipt image and email address.");

                // Check if receiptImage is provided
                if (receiptImage == null)
                {
                    _logger.LogWarning("Receipt image is null or empty.");
                    return new JsonResult(new { success = false, message = "Please provide a receipt image." });
                }

                // Read the uploaded file into a byte array
                using (var memoryStream = new MemoryStream())
                {
                    await receiptImage.CopyToAsync(memoryStream);

                    var image = Image.FromBytes(memoryStream.ToArray());
                    _logger.LogInformation("Converted byte array to image.");

                    _logger.LogInformation("google vision  engine initialized.");
                    var client = ImageAnnotatorClient.Create();
                    var response = await client.DetectTextAsync(image);
                    var imageDataUrl = ConvertToDataUrl(memoryStream.ToArray());
                    var extractedText = string.Join(Environment.NewLine, response.SelectMany(annotation => annotation.Description.Split(new[] { '\n' }, StringSplitOptions.None)));


                    var msg = new EmailMessage();
                    msg.From = "onboarding@resend.dev";
                    msg.To.Add("alonkendler@gmail.com");
                    msg.Subject = "Image to Text";
                    msg.HtmlBody = extractedText;

                    await _resend.EmailSendAsync(msg);

                    return new JsonResult(new
                    {
                        success = true,
                        imageDataUrl,
                        extractedText
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the receipt.");
                return new JsonResult(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
        private string ConvertToDataUrl(byte[] imageBtyes)
        {
            return $"data:image/png;base64, {Convert.ToBase64String(imageBtyes)}";
        }
    }
}