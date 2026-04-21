using System.Text;
using email_constructor.Application.Interfaces;
using email_constructor.Application.Models;
using email_constructor.Cache;
using email_constructor.Domain.Enums;
using email_constructor.Domain.Model;
using email_constructor.Infrastructure.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace email_constructor.Application.Services;

public class ContentBlockService : IContentBlockService
{
    private readonly IContentBlockRepository _contentBlockRepository;
    private readonly IProductCache _productCache;
    private IReadOnlyList<Product> _products;
    
    public ContentBlockService(IContentBlockRepository contentBlockRepository, IProductCache productCache)
    {
        _contentBlockRepository = contentBlockRepository;
        _productCache = productCache;
    }
    
    public async Task<List<RenderedBlock>> GetRenderedBlocksAsync(ContentData contentData, CancellationToken ct)
    {
        var uniqueBlockTypes = contentData.Blocks.Select(b => b.Type).ToHashSet();
        var blockTypes = uniqueBlockTypes.ToList();

        if (!string.IsNullOrEmpty(contentData.ProductCacheKey))
            _products = await _productCache.TryGetProducts(contentData.ProductCacheKey, ct);
        
        var defaultBlocks = await _contentBlockRepository.GetDefaultBlocks(contentData.StoreId, blockTypes);
        var contentBlocksDictionary = defaultBlocks.ToDictionary(b => b.Type);
        
        var blockWrappers = await _contentBlockRepository.GetBlockWrappers();
        var blockWrappersDictionary = blockWrappers.ToDictionary(w => w.WrapperType);
        
        var defaultBlocksData = await _contentBlockRepository.GetDefaultBlocksData(contentData.StoreId, contentData.LanguageId, blockTypes);
        var defaultBlocksDataDictionary = defaultBlocksData.ToDictionary(b=> b.Type);

        FillDefaultPayload(defaultBlocks, defaultBlocksDataDictionary);
        
        var blockToRender = contentData.Blocks
            .Select(block => new ContentBlock
            {
                Type = block.Type,
                Payload = FillPayload(contentBlocksDictionary[block.Type].Payload, block.Payload),
                Html = GetLocalization(contentBlocksDictionary[block.Type].Localizations, contentData.LanguageId)
            }).ToList();
        
        var renderedBlocks = blockToRender
            .Select(contentBlock => RenderBlock(
                contentBlock, 
                defaultBlocksDataDictionary[contentBlock.Type],
                blockWrappersDictionary[WrapperTypes.Default])
            ).ToList();
        
        return renderedBlocks;
    }

    private string GetLocalization(List<Localization> localizations, string languageId) => localizations.First(l => l.LanguageId.Contains(languageId)).Html;
    
    private static Dictionary<string, string>? FillPayload(Dictionary<string, string>? defaultPayload, Dictionary<string, string>? sourcePayload)
    {
        if (sourcePayload is null || sourcePayload.Count == 0) return defaultPayload;
        
        foreach (var (key, value) in defaultPayload)
            sourcePayload.TryAdd(key, value);
        
        return sourcePayload;
    }

    private static void FillHtmlFromPayload(ContentBlock block)
    {
        var result = new StringBuilder(block.Html);
        
        foreach (var (key, value) in block.Payload)
            result.Replace($"#{{{key}}}", value);
        
        block.Html = result.ToString();
    }

    private static void FillDefaultPayload(List<DefaultBlock>? defaultBlocks, Dictionary<string,DefaultBlockData>? defaultBlocksDataDictionary)
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
        var isSpecialBlock = block.Type is "header" or "footer";
    
        if (isSpecialBlock)
            FillHtmlFromPayload(block);
    
        var html = isSpecialBlock 
            ? wrapper.WrapperHtml.Replace("#{content}", block.Html)
            : block.Html;
    
        return new RenderedBlock
        {
            Id = Guid.NewGuid().ToString(),
            Type = block.Type,
            Html = html,
            Payload = block.Payload
        };
    }

    private static List<string> GetBlockTypes(HashSet<string> blockTypes, bool isDefault)
    {
        return isDefault ? blockTypes.Select(b => $"default_{b}").ToList() : blockTypes.ToList();
    }
}