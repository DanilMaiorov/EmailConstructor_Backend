using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace email_constructor.Common.Model;

public class ContentBlockWrapper
{
    /// <summary>
    /// Идентификатор блока.
    /// </summary>
    [BsonId] 
    public ObjectId Id { get; set; }
    
    /// <summary>
    /// Идентификатор блока.
    /// </summary>
    public ContentBlock Block { get; set; }
    
    /// <summary>
    /// Идентификатор блока.
    /// </summary>
    public bool IsVisible { get; set; } = true;
}