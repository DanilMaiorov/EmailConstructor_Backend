using email_constructor.Domain.Enums;

namespace email_constructor.Domain.Exceptions;

/// <summary>
/// Кастомные исключения, связанные с обработкой запросов конструктора писем.
/// </summary>
public static class EmailConstructorExceptions
{
    public class InvalidRequestException(string message) : Exception(message);
    
    
    /// <summary>
    /// Шаблон обязателен для выбранного режима работы.
    /// </summary>
    public class TemplateTypeRequiredException()
        : InvalidRequestException(
            $"Для шаблонного письма нужен templateType.");
    
    
    /// <summary>
    /// Шаблон не найден.
    /// </summary>
    /// <param name="type">Тип шаблона.</param>
    public class TemplateNotFoundException(TemplateType type)
        : InvalidRequestException(
            $"Неизвестный templateType '{type}'.");
    

    /// <summary>
    /// Товары обязательны для выбранного шаблона.
    /// </summary>
    /// <param name="type">Тип шаблона.</param>
    public class ProductsRequiredException(TemplateType type)
        : InvalidRequestException(
            $"Для шаблона {type} нужен файл с продуктами (productCacheKey).");
}