using System.ComponentModel.DataAnnotations;

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
    
    /// <summary>
    /// Список блоков.
    /// </summary>
    [Required] 
    public List<Block> Blocks { get; init; }
    
    /// <summary>
    /// Атрибут шаблонного блока или блока конструктора.
    /// </summary>
    [Required] 
    public bool IsDefaultBlock { get; init; }
}