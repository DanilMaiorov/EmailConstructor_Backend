using System.Text;
using email_constructor.Application.Interfaces;
using email_constructor.Application.Models;
using email_constructor.Cache;
using email_constructor.Domain;
using email_constructor.Domain.Enums;
using email_constructor.Domain.Exceptions;
using email_constructor.Domain.Model;
using email_constructor.Infrastructure.Interfaces;

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
        EnsureProductsProvided(contentData);
        
        IReadOnlyList<Product> products;
        
        if (!string.IsNullOrEmpty(contentData.ProductCacheKey))
            products = await _productCache.TryGetProducts(contentData.ProductCacheKey, ct);
        
        var blocks = await _contentBlockRepository.GetBlocks(contentData.StoreId, contentData.Blocks);
        var contentBlocksDictionary = blocks.ToDictionary(b => b.Key);
        
        var blockWrappers = await _contentBlockRepository.GetBlockWrappers();
        var blockWrappersDictionary = blockWrappers.ToDictionary(w => w.WrapperType);
        
        var blockToRender = contentData.Blocks
            .Select(block => 
            {
                var localization = GetLocalization(contentBlocksDictionary[block.Key].Localizations, contentData.LanguageId);
                
                return new ContentBlock
                {
                    Key = block.Key,
                    Type = block.Type,
                    Variant = block.Variant,
                    Payload = FillPayload(localization.Payload, block.Payload),
                    Html = localization.Html,
                    Css = localization.Css
                };
            }).ToList();
        
        var renderedBlocks = blockToRender
            .Select(contentBlock => RenderBlock(
                contentBlock, 
                blockWrappersDictionary[WrapperTypes.Default])
            ).ToList();
        
        return renderedBlocks;
    }

    private static Localization GetLocalization(List<Localization> localizations, string languageId) => localizations.First(l => l.LanguageId.Contains(languageId));
    
    private static Dictionary<string, string> FillPayload(Dictionary<string, string>? defaultPayload, Dictionary<string, string>? sourcePayload)
    {
        var result = sourcePayload is null
            ? new Dictionary<string, string>()
            : new Dictionary<string, string>(sourcePayload);

        if (defaultPayload is null) return result;

        foreach (var (key, value) in defaultPayload)
        {
            if (!result.TryGetValue(key, out var sourceValue) || string.IsNullOrWhiteSpace(sourceValue))
                result[key] = value;
        }
        
        return result;
        
        //todo добавить информирование о том, какие поля были заполнены из дефолтных
    }

    private static void FillHtmlFromPayload(ContentBlock block)
    {
        var result = new StringBuilder(block.Html);
        
        foreach (var (key, value) in block.Payload)
            result.Replace($"#{{{key}}}", value);
        
        block.Html = result.ToString();
    }

    public RenderedBlock RenderBlock(ContentBlock block, /*DefaultBlockData defaultData,*/ BlockWrapper wrapper)
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
    
    /// <summary>
    /// Проверяет наличие обязательных данных для шаблона.
    /// </summary>
    private static void EnsureProductsProvided(ContentData data)
    {
        if (data.IsConstructor) return;

        var type = data.TemplateType 
                   ?? throw new EmailConstructorExceptions.TemplateTypeRequiredException();

        if (TemplateRules.RequiresProducts(type) && string.IsNullOrEmpty(data.ProductCacheKey))
            throw new EmailConstructorExceptions.ProductsRequiredException(type);
    }
}