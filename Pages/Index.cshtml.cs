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

        public IndexModel(IConfiguration configuration, ILogger<IndexModel> logger, IImageProcessingService imageProcessingService, IEmailService emailService)
        {
            _configuration = configuration;
            _logger = logger;
            _imageProcessingService = imageProcessingService;
            _emailService = emailService;
        }

        public async Task<IActionResult> OnPostAsync(IFormFile receiptImage, string emailAddress)
        {
            try
            {
                _logger.LogInformation("Received receipt image and email address.");

                // Check if receiptImage and emailAddress are provided
                if (receiptImage == null || string.IsNullOrWhiteSpace(emailAddress))
                {
                    _logger.LogWarning("Receipt image or email address is null or empty.");
                    return new JsonResult(new { Success = false, Message = "Please provide a receipt image and a valid email address." });
                }

                // Read the uploaded file into a byte array
                using var memoryStream = new MemoryStream();
                await receiptImage.CopyToAsync(memoryStream);
                var imageBytes = memoryStream.ToArray();

                // Process the image using an image processing service
                var (extractedText, imageDataUrl) = await _imageProcessingService.ProcessImageAsync(imageBytes);

                // Send the extracted text via email using an email service
                await _emailService.SendEmailAsync(
                    emailAddress,
                    "Image to Text email - test",
                    extractedText,
                    _configuration["emailSender"]);

                return new JsonResult(new { Success = true, ExtractedText = extractedText, ImageDataUrl = imageDataUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the receipt.");
                return new JsonResult(new { Success = false, Message = ex.Message });
            }
        }
    }
}