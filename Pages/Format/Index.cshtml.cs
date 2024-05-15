using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using summa_task.Services;

namespace summa_task.Pages
{
    public class FormatModel : PageModel
    {
        private readonly FormattingService _formattingService;
        private readonly ILogger<FormatModel> _logger;

        public FormatModel(FormattingService formattingService, ILogger<FormatModel> logger)
        {
            _formattingService = formattingService;
            _logger = logger;
        }

        public async Task<IActionResult> OnPostAsync([FromBody] InputModel input)
        {
            _logger.LogInformation("Received request to format text.");

            if (string.IsNullOrWhiteSpace(input?.OcrText))
            {
                _logger.LogWarning("OCR text is required.");
                return BadRequest(new { success = false, message = "OCR text is required." });
            }

            try
            {
                _logger.LogInformation("Formatting text: {OcrText}", input.OcrText);
                string formattedText = await _formattingService.FormatInputAsync(input.OcrText);
                return new JsonResult(new { success = true, formattedText = formattedText });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while formatting the text.");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        public class InputModel
        {
            public string OcrText { get; set; }
        }
    }
}
