using System.Text;
using email_constructor.Application.Interfaces;
using email_constructor.Application.Models;
using email_constructor.Domain.Enums;
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
        // var blockTypes = GetBlockTypes(uniqueBlockTypes, contentData.IsDefaultBlock);
        var blockTypes = uniqueBlockTypes.ToList();
        
        var defaultBlocks = await _contentBlockRepository.GetDefaultBlocks(contentData.StoreId, blockTypes);
        var contentBlocksDictionary = defaultBlocks.ToDictionary(b => b.Type);
        
        var blockWrappers = await _contentBlockRepository.GetBlockWrappers();
        var blockWrappersDictionary = blockWrappers.ToDictionary(w => w.WrapperType);
        
        var defaultBlocksData = await _contentBlockRepository.GetDefaultBlocksData(contentData.StoreId, contentData.LanguageId, blockTypes);
        var defaultBlocksDataDictionary = defaultBlocksData.ToDictionary(b=> b.Type);

        FillDefaultPayload(defaultBlocks, defaultBlocksDataDictionary);
        
        
        var s = contentData.Blocks.Select(block => new ContentBlock()
        {
            Type = block.Type,
            Payload = FillPayload(contentBlocksDictionary[block.Type].Payload, block.Payload),
            Html = GetLocalization(contentBlocksDictionary[block.Type].Localizations, contentData.LanguageId)
        }).ToList();
        
        
        ;
        var renderedBlocks = s.Select(contentBlock => 
            RenderBlock(
                contentBlock, 
                defaultBlocksDataDictionary[contentBlock.Type],
                blockWrappersDictionary[WrapperTypes.Default])
            ).ToList();
        
        return renderedBlocks;
    }

    private string GetLocalization(List<Localization> localizations, string languageId)
    {
        return localizations.First(l => l.LanguageId.Contains(languageId)).Html;
    }

    private Dictionary<string, string>? FillPayload(Dictionary<string, string>? defaultPayload, Dictionary<string, string>? sourcePayload)
    {
        if (sourcePayload is null || sourcePayload.Count == 0) return defaultPayload;
        
        foreach (var (key, value) in sourcePayload)
            defaultPayload[key] = value;
        
        return defaultPayload;
    }

    private void FillHtmlFromPayload(ContentBlock block)
    {
        var result = new StringBuilder(block.Html);
        foreach (var (key, value) in block.Payload)
        {
            result.Replace($"#{{{key}}}", value);
        }
        block.Html = result.ToString();
    }

    private void FillDefaultPayload(List<DefaultBlock>? defaultBlocks, Dictionary<string,DefaultBlockData>? defaultBlocksDataDictionary)
    {
        var blocks = defaultBlocks?
            .Select(block =>
            {
                if (defaultBlocksDataDictionary?.TryGetValue(block.Type, out var defaultData) == true)
                    block.Payload = new Dictionary<string, string>(defaultData.Payload);
                return block;
            })
            .ToList();
    }

    public RenderedBlock RenderBlock(ContentBlock block, DefaultBlockData defaultData, BlockWrapper wrapper)
    {
        //написать логику рендера блока
        //заполнять блок данными и помещать блок в обёртку
        FillHtmlFromPayload(block);
        
        return new RenderedBlock
        {
            Id = Guid.NewGuid().ToString(),
            Type = block.Type,
            Html = wrapper.WrapperHtml.Replace($"#{{content}}", block.Html),
        };
    }

    private static List<string> GetBlockTypes(HashSet<string> blockTypes, bool isDefault)
    {
        return isDefault ? blockTypes.Select(b => $"default_{b}").ToList() : blockTypes.ToList();
    }
}