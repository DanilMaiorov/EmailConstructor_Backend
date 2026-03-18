namespace email_constructor.Domain.Model;

public class ContentBlock
{
    /// <summary>
    /// Тип блока.
    /// </summary>
    public string Type { get; set; }
    
    /// <summary>
    /// Payload блока.
    /// </summary>
    public Dictionary<string, string>? Payload { get; set; }
    
    /// <summary>
    /// Верстка обёртки.
    /// </summary>
    public string Html { get; set; }
}