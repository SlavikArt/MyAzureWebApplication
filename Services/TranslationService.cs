using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using Microsoft.Extensions.Logging;

namespace WebApplication1.Services
{
    public class TranslationService : ITranslationService
    {
        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TranslationService> _logger;
        private readonly string _key;
        private readonly string _endpoint;
        private readonly string _location;

        public TranslationService(IConfiguration configuration, ApplicationDbContext context, ILogger<TranslationService> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient();
            _context = context;

            _key = configuration["AzureTranslator:Key"];
            _endpoint = configuration["AzureTranslator:Endpoint"];
            _location = configuration["AzureTranslator:Location"];

            _logger.LogInformation("Initializing TranslationService with endpoint: {Endpoint}", _endpoint);
            
            if (string.IsNullOrEmpty(_key))
            {
                _logger.LogError("Azure Translator Key is null or empty");
                throw new ArgumentException("Azure Translator Key is required");
            }
        }

        public async Task<string> TranslateTextAsync(string text, string targetLanguage)
        {
            try
            {
                _logger.LogInformation("Attempting to translate text to {Language}", targetLanguage);

                string route = $"/translate?api-version=3.0&to={targetLanguage}";
                var body = new[] { new { Text = text } };
                var requestBody = JsonSerializer.Serialize(body);

                using var request = new HttpRequestMessage();
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(_endpoint + route);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", _key);
                request.Headers.Add("Ocp-Apim-Subscription-Region", _location);

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Raw translation response: {Response}", result);

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                var translations = JsonSerializer.Deserialize<TranslationResponse[]>(result, options);
                _logger.LogInformation("Deserialized translations: {Translations}", JsonSerializer.Serialize(translations));
                
                if (translations == null || translations.Length == 0)
                {
                    _logger.LogWarning("No translations returned from the service");
                    return text;
                }

                var firstTranslation = translations[0];
                _logger.LogInformation("First translation object: {Translation}", JsonSerializer.Serialize(firstTranslation));

                if (firstTranslation?.Translations == null || firstTranslation.Translations.Count == 0)
                {
                    _logger.LogWarning("No translations found in the response");
                    return text;
                }

                var translatedText = firstTranslation.Translations[0].Text;
                _logger.LogInformation("Extracted translated text: {TranslatedText}", translatedText);

                if (string.IsNullOrEmpty(translatedText))
                {
                    _logger.LogWarning("Translated text is empty");
                    return text;
                }

                _logger.LogInformation("Successfully translated '{OriginalText}' to '{TranslatedText}'", text, translatedText);
                return translatedText;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Translation failed. Error: {Error}", ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during translation");
                throw;
            }
        }

        public async Task<List<string>> GetAvailableLanguagesAsync()
        {
            return new List<string>
            {
                "en",    // English
                "es",    // Spanish
                "fr",    // French
                "de",    // German
                "it",    // Italian
                "pt",    // Portuguese
                "ru",    // Russian
                "zh-Hans", // Chinese (Simplified)
                "ja",    // Japanese
                "ko",    // Korean
                "ar",    // Arabic
                "hi",    // Hindi
                "tr",    // Turkish
                "nl",    // Dutch
                "pl",    // Polish
                "uk",    // Ukrainian
                "cs",    // Czech
                "sv",    // Swedish
                "da",    // Danish
                "fi"     // Finnish
            };
        }

        public async Task SaveTranslationHistoryAsync(string sourceText, string translatedText, string sourceLanguage, string targetLanguage)
        {
            var history = new TranslationHistory
            {
                SourceText = sourceText,
                TranslatedText = translatedText,
                SourceLanguage = sourceLanguage,
                TargetLanguage = targetLanguage,
                TranslationDate = DateTime.UtcNow
            };

            _context.TranslationHistory.Add(history);
            await _context.SaveChangesAsync();
        }

        public async Task<List<TranslationHistory>> GetTranslationHistoryAsync()
        {
            return await _context.TranslationHistory
                .OrderByDescending(h => h.TranslationDate)
                .ToListAsync();
        }
    }

    public class TranslationResponse
    {
        public DetectedLanguage DetectedLanguage { get; set; } = new();
        public List<Translation> Translations { get; set; } = new();
    }

    public class DetectedLanguage
    {
        public string Language { get; set; } = string.Empty;
        public double Score { get; set; }
    }

    public class Translation
    {
        public string Text { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
    }

    public class LanguagesResponse
    {
        public Dictionary<string, LanguageInfo> Translation { get; set; } = new();
    }

    public class LanguageInfo
    {
        public string Name { get; set; } = string.Empty;
        public string NativeName { get; set; } = string.Empty;
        public string Dir { get; set; } = string.Empty;
    }
}