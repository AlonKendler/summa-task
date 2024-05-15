using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using summa_task.Services;

namespace summa_task.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<IndexModel> _logger;
        private readonly IImageProcessingService _imageProcessingService;
        private readonly IEmailService _emailService;
        private readonly FormattingService _formattingService;

        public string ExtractedText { get; set; }
        public string FormattedText { get; set; }
        public bool EmailSent { get; set; }
        public string EmailErrorMessage { get; set; }

        public IndexModel(
            IConfiguration configuration,
            ILogger<IndexModel> logger,
            IImageProcessingService imageProcessingService,
            IEmailService emailService,
            FormattingService formattingService)
        {
            _configuration = configuration;
            _logger = logger;
            _imageProcessingService = imageProcessingService;
            _emailService = emailService;
            _formattingService = formattingService;
        }

        public async Task<IActionResult> OnPostAsync(IFormFile receiptImage, string emailAddress)
        {
            try
            {
                _logger.LogInformation("Received receipt image and email address.");

                // Validate input
                if (receiptImage == null || string.IsNullOrWhiteSpace(emailAddress))
                {
                    _logger.LogWarning("Receipt image or email address is null or empty.");
                    return new JsonResult(new { Success = false, Message = "Please provide a receipt image and a valid email address." });
                }

                // Read the uploaded file into a byte array
                using var memoryStream = new MemoryStream();
                await receiptImage.CopyToAsync(memoryStream);
                var imageBytes = memoryStream.ToArray();

                // Process the image
                var (response, extractedText, imageDataUrl) = await _imageProcessingService.ProcessImageAsync(imageBytes);

                // Format the extracted text
                FormattedText = await _formattingService.FormatInputAsync(extractedText);

                // Send the extracted text via email
                EmailSent = await _emailService.SendEmailAsync(
                    emailAddress,
                    "Image to Text email - test",
                    extractedText,
                    _configuration["emailSender"]);

                // Handle email sending errors
                if (!EmailSent)
                {
                    EmailErrorMessage = "An error occurred while sending the email. Please try again later.";
                }

                ExtractedText = extractedText;

                // Return the JSON response
                return new JsonResult(new
                {
                    Success = true,
                    ExtractedText,
                    FormattedText,
                    ImageDataUrl = imageDataUrl,
                    Response = response,
                    EmailSent,
                    EmailErrorMessage
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the receipt.");
                return new JsonResult(new { Success = false, Message = ex.Message });
            }
        }
    }
}
