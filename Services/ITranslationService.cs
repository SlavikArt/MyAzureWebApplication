namespace WebApplication1.Services
{
    public interface ITranslationService
    {
        Task<string> TranslateTextAsync(string text, string targetLanguage);
        Task<List<string>> GetAvailableLanguagesAsync();
        Task SaveTranslationHistoryAsync(string sourceText, string translatedText, string sourceLanguage, string targetLanguage);
        Task<List<Models.TranslationHistory>> GetTranslationHistoryAsync();
    }
} 