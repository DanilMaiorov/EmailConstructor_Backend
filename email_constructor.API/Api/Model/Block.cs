using System.ComponentModel.DataAnnotations;

namespace email_constructor.Api.Model;

/// <summary>
/// Представляет блок письма, полученный в запросе.
/// </summary>
public class Block
{
    /// <summary>
    /// Тип блока.
    /// </summary>
    public string Type { get; init; }
    
    /// <summary>
    /// Ключ блока.
    /// </summary>
    public string Key { get; init; }
    
    /// <summary>
    /// Вариант блока.
    /// </summary>
    public string Variant { get; init; }
    
    /// <summary>
    /// Payload блока.
    /// </summary>
    public Dictionary<string, string> Payload { get; init; }
}