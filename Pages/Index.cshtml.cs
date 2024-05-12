using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Drawing;
using System.IO;
using Tesseract;

namespace summa_task.Pages;
public class IndexModel : PageModel
{
    private readonly EmailService _emailService;

    public IndexModel(EmailService emailService)
    {
        _emailService = emailService;
    }

    public IActionResult OnPost(Receipt receipt)
    {
        try
        {
            // Convert byte array to Image
            using (var ms = new MemoryStream(receipt.ReceiptImage))
            {
                var image = Image.FromStream(ms);

                // Perform OCR using Tesseract
                using (var engine = new TesseractEngine(@"./tessdata", "heb", EngineMode.Default))
                {
                    using (var img = PixConverter.ToPix(image))
                    {
                        using (var page = engine.Process(img))
                        {
                            var ocrOutput = page.GetText();

                            // Send OCR output to email
                            _emailService.SendEmailWithOcrOutput(receipt.EmailAddress, ocrOutput);
                        }
                    }
                }
            }

            return new JsonResult(new { success = true, message = $"Email sent to: {receipt.EmailAddress}" });
        }
        catch (Exception ex)
        {
            return new JsonResult(new { success = false, message = ex.Message });
        }
    }
}