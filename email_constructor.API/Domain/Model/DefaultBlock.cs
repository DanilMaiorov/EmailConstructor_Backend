using email_constructor.Domain.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace email_constructor.Domain.Model;

/// <summary>
/// Базовая модель блока конструктора.
/// </summary>
public class DefaultBlock
{
    /// <summary>
    /// Название коллекции.
    /// </summary>
    public static string Collection => nameof(DefaultBlock);
    
    /// <summary>
    /// Идентификатор блока.
    /// </summary>
    [BsonId]
    public ObjectId Id { get; set; }
    
    /// <summary>
    /// Тип блока.
    /// </summary>
    public string Type { get; set; }
    
    /// <summary>
    /// Payload блока.
    /// </summary>
    public Dictionary<string, string> Payload { get; set; }
    
    /// <summary>
    /// Верстка блока.
    /// </summary>
    public List<Localization> Localizations { get; set; }
    
    /// <summary>
    /// Тип обертки блока.
    /// </summary>
    public WrapperTypes WrapperType { get; set; }
}