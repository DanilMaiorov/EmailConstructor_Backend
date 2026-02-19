using email_constructor.Application.Models;
using email_constructor.Common.Model;

namespace email_constructor.Api.Extensions;

public interface IMapper
{
    ContentData MapToContentData(GetEmailContentBlockRequest request);
    GetEmailContentBlockResponse MapToResponse(List<BlockData> blocks);
}
public class Mapper : IMapper
{
    public ContentData MapToContentData(GetEmailContentBlockRequest request) =>
        new()
        {
            LanguageId = request.LanguageId,
            StoreId = request.StoreId,
            Blocks = request.Blocks.Select(MapToBlockData).ToList(),
        };

    private static BlockData MapToBlockData(Block block) => 
        new()
        {
            Type = block.Type,
            Payload = block.Payload.ToDictionary(),
        };
    
    public GetEmailContentBlockResponse MapToResponse(List<BlockData> blocks) =>
        new()
        {
            Blocks = { blocks.Select(MapToBlock) }
        };
    
    
    private static Block MapToBlock(BlockData blockData) => 
        new()
        {
            Type = blockData.Type, 
            Payload = { blockData.Payload }
        };
}