using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace email_constructor.Common.Model;

/// <summary>
/// Базовая модель блока конструктора.
/// </summary>
public class ContentBlock
{
    /// <summary>
    /// Название коллекции.
    /// </summary>
    public static string Collection => nameof(ContentBlock);
    
    /// <summary>
    /// Идентификатор блока.
    /// </summary>
    [BsonId]
    public ObjectId Id { get; set; }
    
    /// <summary>
    /// Идентификатор магазина. 
    /// </summary>
    public string StoreId { get; set; }
    
    /// <summary>
    /// Ключ блока.
    /// </summary>
    public string Key { get; set; }
    
    /// <summary>
    /// Тип блока.
    /// </summary>
    public string Type { get; set; }
    
    /// <summary>
    /// Верстка блока.
    /// </summary>
    public Dictionary<string, string> Payload { get; set; }
    
    /// <summary>
    /// Верстка блока.
    /// </summary>
    public List<Localization> Localizations { get; set; }
}