namespace email_constructor.Application.Models;

public class BlockData
{
    /// <summary>
    /// Ключ блока. 
    /// </summary>
    public string Key { get; init; }
    
    /// <summary>
    /// Тип блока. 
    /// </summary>
    public string Type { get; init; }
    
    /// <summary>
    /// Вариант блока. 
    /// </summary>
    public string Variant { get; init; }
    
    /// <summary>
    /// Payload блока. 
    /// </summary>
    public Dictionary<string, string> Payload { get; init; }
}