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
    public ContentData MapToContentData(GetEmailContentBlockRequest request)
    {
        return new ContentData
        {
            LanguageId = request.LanguageId,
            StoreId = request.StoreId,
            Blocks = request.Blocks.Select(MapToBlockData).ToList(),
        };
    }

    private static BlockData MapToBlockData(Block block)
    {
        return new BlockData
        {
            Type = block.Type,
            Payload = block.Payload.ToDictionary(),
        };
    }
    
    public GetEmailContentBlockResponse MapToResponse(List<BlockData> blocks)
    {
        return new GetEmailContentBlockResponse()
        {
            Blocks = { blocks.Select(MapToBlock) }
        };
    }
    
    private static Block MapToBlock(BlockData blockData)
    {
        return new Block
        {
            Type = blockData.Type, 
            Payload = { blockData.Payload }
        };
    }
}