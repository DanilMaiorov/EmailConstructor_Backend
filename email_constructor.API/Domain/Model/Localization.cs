namespace email_constructor.Domain.Model;

public class Localization
{
    /// <summary>
    /// Идентификатор языка.
    /// </summary>
    public List<string> LanguageId { get; set; }
    
    /// <summary>
    /// Верстка элемента.
    /// </summary>
    public string Html { get; set; }
}