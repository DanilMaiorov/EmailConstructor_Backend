namespace email_constructor.Application.Models;

public class BlockData
{
    /// <summary>
    /// Тип блока. 
    /// </summary>
    public string Type { get; init; }
    
    /// <summary>
    /// Payload блока. 
    /// </summary>
    public Dictionary<string, string> Payload { get; init; }
}