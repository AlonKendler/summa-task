using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Mindee;
using Mindee.Input;
using Mindee.Product.Invoice;

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

        public async Task OnGetAsync(string imagePath = null)
        {
            try
            {
                // If imagePath is provided, use it; otherwise, use a default image path
                var imageFilePath = imagePath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "sample_receipt.png");

                // Load the receipt image
                var imageData = await File.ReadAllBytesAsync(imageFilePath);

                // Perform OCR using the Mindee SDK
                string apiKey = "3816d714700ff0d5dcff9006518ffb9b";
                MindeeClient mindeeClient = new MindeeClient(apiKey);
                var inputSource = new LocalInputSource(imageData);
                var response = await mindeeClient.ParseAsync<InvoiceV4>(inputSource);

                // Extract the OCR text and image data
                OCRData = new ReceiptOCRData
                {
                    Text = response.Document.Inference.Prediction.ToString(),
                    ImageData = imageData
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