namespace email_constructor.Domain.Model;

/// <summary>
/// Модель продукта
/// </summary>
public class Product
{
    /// <summary>
    /// Ссылка
    /// </summary>
    public string Link { get; set; }
    
    /// <summary>
    /// Тип
    /// </summary>
    public string Type { get; set; }
    
    /// <summary>
    /// Бренд
    /// </summary>
    public string Brand { get; set; }
    
    /// <summary>
    /// Модель
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Текстовое описание
    /// </summary>
    public string? Description { get; set; } = null;
    
    /// <summary>
    /// Текущая цена
    /// </summary>
    public string Price { get; set; }

    /// <summary>
    /// Старая цена
    /// </summary>
    public string OldPrice { get; set; }

    /// <summary>
    /// Скю
    /// </summary>
    public string Sku { get; set; }
    
    /// <summary>
    /// Ссылка на изображение
    /// </summary>
    public string Img { get; set; }
}