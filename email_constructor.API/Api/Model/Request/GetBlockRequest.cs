using System.ComponentModel.DataAnnotations;
using email_constructor.Domain.Enums;

namespace email_constructor.Api.Model.Request;

public class GetBlockRequest
{
    /// <summary>
    /// Язык.
    /// </summary>
    [Required] 
    public string LanguageId { get; init; }
    
    /// <summary>
    /// Идентификатор магазина. 
    /// </summary>
    [Required] 
    public string StoreId { get; init; }
    
    // /// <summary>
    // /// Ключ кеша продуктов. 
    // /// </summary>
    // [Required] 
    // public string ProductCacheKey { get; init; }
    //
    // /// <summary>
    // /// Список блоков.
    // /// </summary>
    // [Required] 
    // public List<Block> Blocks { get; init; }
    
    /// <summary>
    /// Список блоков.
    /// </summary>
    [Required] 
    public WrapperTypes WrapperType { get; init; }
    
    /// <summary>
    /// Режим сборки. true — конструктор, false — шаблонное письмо. 
    /// </summary>
    [Required] 
    public bool IsConstructor { get; init; }
}