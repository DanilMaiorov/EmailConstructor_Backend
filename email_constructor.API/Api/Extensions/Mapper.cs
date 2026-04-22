using email_constructor.Application.Models;
using email_constructor.Domain.Model;

namespace email_constructor.Api.Extensions;

public interface IMapper
{
    ContentData MapToContentData(email_constructor.Api.Model.Request.GetEmailTemplateRequest request);
    GetEmailTemplateResponse MapToResponse(List<RenderedBlock> blocks);
}
public class Mapper : IMapper
{
    public ContentData MapToContentData(email_constructor.Api.Model.Request.GetEmailTemplateRequest request) =>
        new()
        {
            LanguageId = request.LanguageId,
            StoreId = request.StoreId,
            ProductCacheKey = request.ProductCacheKey,
            Blocks = request.Blocks.Select(MapToBlockData).ToList(),
            IsDefaultBlock = request.IsDefaultBlock
        };

    private static BlockData MapToBlockData(email_constructor.Api.Model.Block block) => 
        new()
        {
            Key = block.Key,
            Type = block.Type,
            Variant = block.Variant,
            Payload = block.Payload.ToDictionary(),
        };
    
    public GetEmailTemplateResponse MapToResponse(List<RenderedBlock> blocks) =>
        new()
        {
            Blocks = { blocks.Select(MapToBlock) }
        };
    
    private static Block MapToBlock(RenderedBlock renderedBlock) => 
        new()
        {
            Type = renderedBlock.Type,
            Html = renderedBlock.Html,
            Payload = { renderedBlock.Payload }
        };
}