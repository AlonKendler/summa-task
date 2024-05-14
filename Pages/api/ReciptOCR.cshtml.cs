using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Mindee;
using Mindee.Input;
using Mindee.Product.Invoice;
using System;
using System.IO;
using System.Threading.Tasks;

namespace summa_task.Pages.api
{
    public class ReciptOCRModel : PageModel
    {
        private readonly ILogger<ReciptOCRModel> _logger;

        // Declare OCRData as a nullable property
        public ReceiptOCRData? OCRData { get; set; }


        public ReciptOCRModel(ILogger<ReciptOCRModel> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> OnPostAsync(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                ModelState.AddModelError("ImageFile", "Please select a file.");
                return Page();
            }

            try
            {
                // Save the uploaded file to a temporary location
                var filePath = Path.GetTempFileName();
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                await ProcessImageAsync(filePath);

                // Redirect to the GET action to display the result
                return RedirectToPage("/api/ReciptOCR", new { imagePath = filePath });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the receipt.");
                OCRData = new ReceiptOCRData { Text = "Error: " + ex.Message };
                return Page();
            }
        }

        private async Task ProcessImageAsync(string imagePath)
        {
            if (imagePath == null)
            {
                throw new ArgumentNullException(nameof(imagePath), "Image path cannot be null");
            }

            try
            {
                // Perform OCR using the Mindee SDK
                string apiKey = "3816d714700ff0d5dcff9006518ffb9b";
                MindeeClient mindeeClient = new MindeeClient(apiKey);
                var inputSource = new LocalInputSource(imagePath);
                var response = await mindeeClient.ParseAsync<InvoiceV4>(inputSource);

                // Extract the OCR text and image data
                OCRData = new ReceiptOCRData
                {
                    Text = response.ToString(), // Adjust this line to match how Mindee response provides OCR text
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the receipt.");
                OCRData = new ReceiptOCRData { Text = "Error: " + ex.Message };
            }
        }

    }
}
