using email_constructor.Application.Models;
using email_constructor.Domain.Model;

namespace email_constructor.Api.Extensions;

public interface IMapper
{
    ContentData MapToContentData(GetEmailContentBlockRequest request);
    GetEmailContentBlockResponse MapToResponse(List<RenderedBlock> blocks);
}
public class Mapper : IMapper
{
    public ContentData MapToContentData(GetEmailContentBlockRequest request) =>
        new()
        {
            LanguageId = request.LanguageId,
            StoreId = request.StoreId,
            Blocks = request.Blocks.Select(MapToBlockData).ToList(),
            IsDefaultBlock = request.IsDefaultBlock
        };

    private static BlockData MapToBlockData(Block block) => 
        new()
        {
            Type = block.Type,
            Payload = block.Payload.ToDictionary(),
        };
    
    public GetEmailContentBlockResponse MapToResponse(List<RenderedBlock> blocks) =>
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