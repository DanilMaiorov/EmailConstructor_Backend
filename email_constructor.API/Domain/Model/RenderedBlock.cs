namespace email_constructor.Domain.Model;

public class RenderedBlock
{
    /// <summary>
    /// ID блока (нужен, чтобы фронт понимал, какой именно блок редактировать/двигать).
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Тип блока.
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// Готовая верстка: обертка + наполнение. 
    /// </summary>
    public string Html { get; set; }
    
    /// <summary>
    /// Опционально: видимость (если нужно скрывать блок на фронте без удаления).
    /// </summary>
    public bool IsVisible { get; set; } = true;
}