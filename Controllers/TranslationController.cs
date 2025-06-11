using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;
using Microsoft.Extensions.Logging;

namespace WebApplication1.Controllers
{
    public class TranslationController : Controller
    {
        private readonly ITranslationService _translationService;
        private readonly ILogger<TranslationController> _logger;

        public TranslationController(ITranslationService translationService, ILogger<TranslationController> logger)
        {
            _translationService = translationService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var languages = await _translationService.GetAvailableLanguagesAsync();
            ViewBag.Languages = languages;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Translate(string text, string targetLanguage)
        {
            if (string.IsNullOrEmpty(text))
            {
                return BadRequest("Text cannot be empty");
            }

            try
            {
                _logger.LogInformation("Translating text: {Text} to language: {Language}", text, targetLanguage);
                var translatedText = await _translationService.TranslateTextAsync(text, targetLanguage);
                _logger.LogInformation("Translation result: {TranslatedText}", translatedText);

                if (string.IsNullOrEmpty(translatedText))
                {
                    return BadRequest("Translation failed");
                }

                await _translationService.SaveTranslationHistoryAsync(text, translatedText, "auto", targetLanguage);

                return Json(new { translatedText = translatedText });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during translation");
                return StatusCode(500, "An error occurred during translation");
            }
        }

        public async Task<IActionResult> History()
        {
            var history = await _translationService.GetTranslationHistoryAsync();
            return View(history);
        }
    }
} 