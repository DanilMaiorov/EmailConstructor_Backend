using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace email_constructor.Domain.Model;

/// <summary>
/// Модель с данными для дефолтных блоков.
/// </summary>
public class DefaultBlockData
{
    /// <summary>
    /// Название коллекции.
    /// </summary>
    public static string Collection => nameof(DefaultBlockData);
    
    /// <summary>
    /// Идентификатор.
    /// </summary>
    [BsonId] 
    public ObjectId Id { get; set; }
    
    /// <summary>
    /// Идентификатор магазина. 
    /// </summary>
    public string StoreId { get; set; }
    
    /// <summary>
    /// Идентификатор языка. 
    /// </summary>
    public string LanguageId { get; set; }
    
    /// <summary>
    /// Тип блока.
    /// </summary>
    public string Type { get; set; }
    
        
    /// <summary>
    /// Payload блока.
    /// </summary>
    public Dictionary<string, string> Payload { get; set; }
}