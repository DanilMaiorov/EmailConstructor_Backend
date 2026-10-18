namespace email_constructor.Domain.Enums;

/// <summary>
/// Типы загружаемых файлов
/// </summary>
public enum UploadFileType
{
    //Нет файла
    None = 0,
    
    /// <summary>
    /// Excel
    /// </summary>
    Excel = 1,
    
    /// <summary>
    /// Любой другой тип
    /// </summary>
    AnotherType
}