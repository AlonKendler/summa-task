using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Drawing;
using Tesseract;

namespace summa_task.Pages
{
    public class IndexModel : PageModel
    {
        private readonly EmailService _emailService;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(EmailService emailService, ILogger<IndexModel> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }


        public IActionResult OnGet(string id)
        {
            return new JsonResult(new { success = true, mesaage = "onGet" });
        }

        public async Task<IActionResult> OnPostAsync(List<IFormFile> receiptImage, string emailAddress)
        {
            try
            {
                _logger.LogInformation("Received receipt image and email address.");

                // Check if receiptImage is provided
                if (receiptImage == null || receiptImage.Count == 0)
                {
                    _logger.LogWarning("Receipt image is null or empty.");
                    return new JsonResult(new { success = false, message = "Please provide a receipt image." });
                }

                // Read the uploaded file into a byte array
                using (var memoryStream = new MemoryStream())
                {
                    await receiptImage[0].OpenReadStream().CopyToAsync(memoryStream);
                    var receiptImageData = memoryStream.ToArray();

                    // Create a Receipt object with the image data and email address
                    var receipt = new Receipt
                    {
                        ReceiptImage = receiptImageData,
                        EmailAddress = emailAddress
                    };

                    // Convert byte array to Image
                    using (var ms = new MemoryStream(receipt.ReceiptImage))
                    {
                        var image = Image.FromStream(ms);
                        _logger.LogInformation("Converted byte array to image.");

                        // Perform OCR using Tesseract
                        using (var engine = new TesseractEngine(@"./tessdata", "heb", EngineMode.Default))
                        {
                            _logger.LogInformation("Tesseract engine initialized.");

                            using (var img = new System.Drawing.Bitmap(image))
                            {
                                _logger.LogInformation("Created bitmap from image.");

                                using (var page = engine.Process(img))
                                {
                                    var GetMeanConfidence = string.Format("{0:P}", page.GetMeanConfidence());
                                    _logger.LogInformation($"OCR mean confidence: {GetMeanConfidence}");

                                    var ocrOutput = page.GetText();
                                    _logger.LogInformation("OCR output retrieved.");

                                    // Send OCR output to email
                                    _emailService.SendEmailWithOcrOutput(receipt.EmailAddress, ocrOutput);
                                    _logger.LogInformation("Email sent successfully.");
                                }
                            }
                        }
                    }
                }

                return new JsonResult(new { success = true, message = $"Email sent to: {emailAddress}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the receipt.");
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}