using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace email_constructor.Common.Model;

/// <summary>
/// Базовая модель блока конструктора.
/// </summary>
public abstract class ContentBlock
{
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
    public string Html { get; set; }
    
    /// <summary>
    /// Верстка блока.
    /// </summary>
    public Dictionary<string, string> Payload { get; set; }
}