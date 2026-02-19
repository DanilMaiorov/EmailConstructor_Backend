using email_constructor.Application.Interfaces;
using email_constructor.Application.Models;
using email_constructor.Common.Model;
using email_constructor.Infrastructure.Interfaces;

namespace email_constructor.Application.Services;

public class ContentBlockService : IContentBlockService
{
    private readonly IContentBlockRepository _contentBlockRepository;

    public ContentBlockService(IContentBlockRepository contentBlockRepository)
    {
        _contentBlockRepository = contentBlockRepository;
    }
    
    public async Task<List<ContentBlock>> GetBlocksAsync(ContentData contentData)
    {
        var uniqueBlockTypes = contentData.Blocks.Select(b => b.Type).ToHashSet();
        
        var blockTypes = GetBlockTypes(uniqueBlockTypes, contentData.IsDefaultBlock);
        
        var blocks = await _contentBlockRepository.GetContentBlocks(contentData.StoreId, blockTypes);
        ;
        return blocks;
    }

    public Task<ContentBlockWrapper> GetWrappedBlockAsync(string id, string storeId)
    {
        ;
        throw new NotImplementedException();
    }

    private static List<string> GetBlockTypes(HashSet<string> blockTypes, bool isDefault)
    {
        return isDefault ? blockTypes.Select(b => $"default_{b}").ToList() : blockTypes.ToList();
    }
}