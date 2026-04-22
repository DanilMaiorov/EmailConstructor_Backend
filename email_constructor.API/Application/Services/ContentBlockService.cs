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
    
    
    public ContentBlockService(IContentBlockRepository contentBlockRepository, IProductCache productCache)
    {
        _contentBlockRepository = contentBlockRepository;
        _productCache = productCache;
    }
    
    public async Task<List<RenderedBlock>> GetRenderedBlocksAsync(ContentData contentData, CancellationToken ct)
    {
        IReadOnlyList<Product> products;

        if (!string.IsNullOrEmpty(contentData.ProductCacheKey))
            products = await _productCache.TryGetProducts(contentData.ProductCacheKey, ct);
        
        var blocks = await _contentBlockRepository.GetBlocks(contentData.StoreId, contentData.Blocks);
        var contentBlocksDictionary = blocks.ToDictionary(b => b.Key);
        
        var blockWrappers = await _contentBlockRepository.GetBlockWrappers();
        var blockWrappersDictionary = blockWrappers.ToDictionary(w => w.WrapperType);
        
        var defaultBlocksData = await _contentBlockRepository.GetBlocksDefaultData(contentData.StoreId, contentData.LanguageId, contentData.Blocks);
        var defaultBlocksDataDictionary = defaultBlocksData.ToDictionary(b=> b.Key);

        FillDefaultPayload(blocks, defaultBlocksDataDictionary);
        
        var blockToRender = contentData.Blocks
            .Select(block => 
            {
                var localization = GetLocalization(contentBlocksDictionary[block.Key].Localizations, contentData.LanguageId);
                return new ContentBlock
                {
                    Key = block.Key,
                    Type = block.Type,
                    Variant = block.Variant,
                    Payload = FillPayload(contentBlocksDictionary[block.Key].Payload, block.Payload),
                    Html = localization.Html,
                    Css = localization.Css
                };
            }).ToList();
        
        var renderedBlocks = blockToRender
            .Select(contentBlock => RenderBlock(
                contentBlock, 
                defaultBlocksDataDictionary[contentBlock.Key],
                blockWrappersDictionary[WrapperTypes.Default])
            ).ToList();
        
        return renderedBlocks;
    }

    private static Localization GetLocalization(List<Localization> localizations, string languageId) => localizations.First(l => l.LanguageId.Contains(languageId));
    
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
                if (defaultBlocksDataDictionary?.TryGetValue(block.Key, out var defaultData) == true)
                    block.Payload = new Dictionary<string, string>(defaultData.Payload);
                return block;
            })
            .ToList();
    }

    public RenderedBlock RenderBlock(ContentBlock block, DefaultBlockData defaultData, BlockWrapper wrapper)
    {
        var isSpecialBlock = block.Key is "header" or "footer" or "banner";
        
         FillHtmlFromPayload(block);
    
        var html = isSpecialBlock 
            ? block.Html
            : wrapper.WrapperHtml.Replace("#{content}", block.Html);
    
        return new RenderedBlock
        {
            Id = Guid.NewGuid().ToString(),
            Key = block.Key,
            Type = block.Type,
            Variant = block.Variant,
            Html = html,
            Css = block.Css,
            Payload = block.Payload
        };
    }
}