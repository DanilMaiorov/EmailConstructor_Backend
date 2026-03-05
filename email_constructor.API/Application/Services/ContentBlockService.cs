using email_constructor.Application.Interfaces;
using email_constructor.Application.Models;
using email_constructor.Domain.Model;
using email_constructor.Infrastructure.Interfaces;

namespace email_constructor.Application.Services;

public class ContentBlockService : IContentBlockService
{
    private readonly IContentBlockRepository _contentBlockRepository;

    public ContentBlockService(IContentBlockRepository contentBlockRepository)
    {
        _contentBlockRepository = contentBlockRepository;
    }
    
    public async Task<List<RenderedBlock>> GetRenderedBlocksAsync(ContentData contentData)
    {
        var uniqueBlockTypes = contentData.Blocks.Select(b => b.Type).ToHashSet();
        var blockTypes = GetBlockTypes(uniqueBlockTypes, contentData.IsDefaultBlock);
        
        var contentBlocks = await _contentBlockRepository.GetContentBlocks(contentData.StoreId, blockTypes);
        var contentBlockWrappers = await _contentBlockRepository.GetContentBlockWrappers();
        var contentBlockWrappersDictionary = contentBlockWrappers.ToDictionary(w => w.WrapperType);
        
        var renderedBlocks = contentBlocks.Select(block => RenderBlock(block, contentBlockWrappersDictionary[block.WrapperType])).ToList();
        
        return renderedBlocks;
    }

    public RenderedBlock RenderBlock(ContentBlock block, BlockWrapper wrapper)
    {
        var block1 = block; 
        var wrapper1 = wrapper; 
        
        return new RenderedBlock();
    }

    private static List<string> GetBlockTypes(HashSet<string> blockTypes, bool isDefault)
    {
        return isDefault ? blockTypes.Select(b => $"default_{b}").ToList() : blockTypes.ToList();
    }
}