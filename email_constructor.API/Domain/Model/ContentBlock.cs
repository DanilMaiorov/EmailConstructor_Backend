namespace email_constructor.Domain.Model;

public class ContentBlock
{
    /// <summary>
    /// Ключ блока.
    /// </summary>
    public string Key { get; set; }
    
    /// <summary>
    /// Тип блока.
    /// </summary>
    public string Type { get; set; }
    
    /// <summary>
    /// Вариант блока.
    /// </summary>
    public string Variant { get; set; }
    
    /// <summary>
    /// Payload блока.
    /// </summary>
    public Dictionary<string, string> Payload { get; set; }
    
    /// <summary>
    /// Верстка блока.
    /// </summary>
    public string Html { get; set; }
    
    /// <summary>
    /// Мобильные стили блока.
    /// </summary>
    public string Css { get; set; }
}