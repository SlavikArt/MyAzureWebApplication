using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class TranslationHistory
    {
        public int Id { get; set; }

        [Required]
        public string SourceText { get; set; }

        [Required]
        public string TranslatedText { get; set; }

        [Required]
        public string SourceLanguage { get; set; }

        [Required]
        public string TargetLanguage { get; set; }

        public DateTime TranslationDate { get; set; } = DateTime.UtcNow;
    }
} 