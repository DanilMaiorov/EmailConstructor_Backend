using email_constructor.Application.Models;
using email_constructor.Domain.Enums;
using email_constructor.Domain.Exceptions;
using email_constructor.Domain.Model;

namespace email_constructor.Api.Extensions;

public interface IMapper
{
    ContentData MapToContentData(email_constructor.Api.Model.Request.GetEmailTemplateRequest request);
    GetEmailTemplateResponse MapToResponse(List<RenderedBlock> blocks);
}
public class Mapper : IMapper
{
    /// <summary>
    /// Мапит запрос в модель данных для обработки шаблона.
    /// </summary>
    public ContentData MapToContentData(email_constructor.Api.Model.Request.GetEmailTemplateRequest request) =>
        new()
        {
            LanguageId = request.LanguageId,
            StoreId = request.StoreId,
            Blocks = request.Blocks.Select(MapToBlockData).ToList(),
            IsConstructor = request.IsConstructor,
            TemplateType = ParseTemplateType(request.IsConstructor, request.TemplateType),
            ProductCacheKey = request.ProductCacheKey,
        };

    /// <summary>
    /// Мапит запрос в модель данных для обработки шаблона.
    /// </summary>
    private static BlockData MapToBlockData(email_constructor.Api.Model.Block block) => 
        new()
        {
            Key = block.Key,
            Type = block.Type,
            Variant = block.Variant,
            Payload = block.Payload.ToDictionary(),
        };
    
    /// <summary>
    /// Мапит отрендеренные блоки в ответ API.
    /// </summary>
    public GetEmailTemplateResponse MapToResponse(List<RenderedBlock> blocks) =>
        new()
        {
            Blocks = { blocks.Select(MapToBlock) }
        };
    
    /// <summary>
    /// Мапит отрендеренный блок в модель ответа API.
    /// </summary>
    private static Block MapToBlock(RenderedBlock renderedBlock) => 
        new()
        {
            Type = renderedBlock.Type,
            Html = renderedBlock.Html,
            Payload = { renderedBlock.Payload }
        };
    
    /// <summary>
    /// Проверяет наличие обязательных данных для шаблона.
    /// </summary>
    private static TemplateType? ParseTemplateType(bool isConstructor, string? value)
    {
        if (isConstructor) return null;
        
        if (string.IsNullOrWhiteSpace(value)) throw new EmailConstructorExceptions.InvalidRequestException("Для шаблонного письма нужен templateType.");
        
        if (Enum.TryParse(value, ignoreCase: true, out TemplateType templateType)
            && Enum.IsDefined(templateType)
            && string.Equals(templateType.ToString(), value, StringComparison.OrdinalIgnoreCase))
            return templateType;

        throw new EmailConstructorExceptions.TemplateNotFoundException(templateType);
    }
}