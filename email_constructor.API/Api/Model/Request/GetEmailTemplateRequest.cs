using System.ComponentModel.DataAnnotations;
using email_constructor.Domain.Enums;

namespace email_constructor.Api.Model.Request;

public class GetEmailTemplateRequest
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
    
    /// <summary>
    /// Список блоков.
    /// </summary>
    [Required] 
    public List<Block> Blocks { get; init; }
        
    /// <summary>
    /// Режим сборки. true — конструктор, false — шаблонное письмо. 
    /// </summary>
    [Required] 
    public bool IsConstructor { get; init; }
    
    /// <summary>
    /// Тип шаблонного письма. Заполнен только если IsConstructor = false.
    /// </summary>
    public string? TemplateType { get; init; }
    
    /// <summary>
    /// Список блоков.
    /// </summary>
    [Required] 
    public WrapperTypes WrapperType { get; init; }
    
    /// <summary>
    /// Ключ кеша продуктов. 
    /// </summary>
    public string ProductCacheKey { get; init; }
}