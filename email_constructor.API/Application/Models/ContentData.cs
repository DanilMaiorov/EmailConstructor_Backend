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
}