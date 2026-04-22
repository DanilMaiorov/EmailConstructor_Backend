namespace email_constructor.Domain.Model;

public class Localization
{
    /// <summary>
    /// Идентификатор языка.
    /// </summary>
    public string LanguageId { get; set; }
    
    /// <summary>
    /// Верстка элемента.
    /// </summary>
    public string Html { get; set; }
    
    /// <summary>
    /// Мобильные стили элемента.
    /// </summary>
    public string Css { get; set; }
}