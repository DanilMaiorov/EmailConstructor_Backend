using email_constructor.Domain.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace email_constructor.Domain.Model;

public class BlockWrapper
{
    /// <summary>
    /// Название коллекции.
    /// </summary>
    public static string Collection => nameof(BlockWrapper);
    
    /// <summary>
    /// Идентификатор блока.
    /// </summary>
    [BsonId] 
    public ObjectId Id { get; set; }
    
    /// <summary>
    /// Верстка обёртки.
    /// </summary>
    public string WrapperHtml { get; set; }
    
    /// <summary>
    /// Тип обертки блока.
    /// </summary>
    public WrapperTypes WrapperType { get; set; }
}