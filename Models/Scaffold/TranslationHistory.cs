using System;
using System.Collections.Generic;

namespace WebApplication1.Models.Scaffold;

public partial class TranslationHistory
{
    public int Id { get; set; }

    public string SourceText { get; set; } = null!;

    public string TranslatedText { get; set; } = null!;

    public string SourceLanguage { get; set; } = null!;

    public string TargetLanguage { get; set; } = null!;

    public DateTime TranslationDate { get; set; }
}
