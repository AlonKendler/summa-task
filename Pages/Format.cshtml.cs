using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using summa_task.Services;

public class FormatModel : PageModel
{
    private readonly FormattingService _formattingService;
    private readonly ILogger<FormatModel> _logger;

    public FormatModel(FormattingService formattingService, ILogger<FormatModel> logger)
    {
        _formattingService = formattingService;
        _logger = logger;
    }

    [BindProperty]
    public string? ocrText { get; set; }
    public async Task<IActionResult> OnPostAsync()
    {
        _logger.LogInformation("Request Body: " + Request.Body);
        _logger.LogInformation($"Received OCR text: {ocrText}");
        try
        {
            var formattedText = await _formattingService.FormatInputAsync(ocrText);
            return new JsonResult(new { Success = true, FormattedText = formattedText });
        }
        catch (Exception ex)
        {
            return new JsonResult(new { Success = false, Message = ex.Message });
        }
    }
}