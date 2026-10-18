using email_constructor.Domain.Enums;

namespace email_constructor.Application.Models;

public class ContentData
{
    /// <summary>
    /// Язык.
    /// </summary>
    public string LanguageId { get; init; }
    
    /// <summary>
    /// Идентификатор магазина. 
    /// </summary>
    public string StoreId { get; init; }
    
    /// <summary>
    /// Cписок блоков. 
    /// </summary>
    public List<BlockData> Blocks { get; set; }
    
    /// <summary>
    /// Режим сборки. true — конструктор, false — шаблонное письмо. 
    /// </summary>
    public bool IsConstructor { get; init; }
    
    /// <summary>
    /// Тип шаблонного письма. Заполнен только если IsConstructor = false.
    /// </summary>
    public TemplateType? TemplateType { get; init; }
    
    /// <summary>
    /// Ключ кэша с продуктами
    /// </summary>
    public string ProductCacheKey { get; init; }
}