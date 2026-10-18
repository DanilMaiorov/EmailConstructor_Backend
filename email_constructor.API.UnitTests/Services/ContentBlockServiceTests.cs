using email_constructor.Api.Extensions;
using email_constructor.Api.Model;
using email_constructor.Api.Model.Request;
using email_constructor.Application.Models;
using email_constructor.Application.Services;
using email_constructor.Cache;
using email_constructor.Domain.Enums;
using email_constructor.Domain.Model;
using email_constructor.Infrastructure.Interfaces;
using Moq;

namespace Tests.Services;

public class ContentBlockServiceTests
{
    private readonly Mock<IContentBlockRepository> _contentBlockRepository;
    private readonly Mock<IProductCache> _productCache;
    private readonly IMapper _mapper;
    private readonly ContentBlockService _sut;
    
    public ContentBlockServiceTests()
    {
        _mapper = new Mapper();
        _contentBlockRepository = new Mock<IContentBlockRepository>();
        _productCache = new Mock<IProductCache>();
        
        _sut = new ContentBlockService(
            _contentBlockRepository.Object, 
            _productCache.Object);
    }
    
    [Fact]
    public async Task GetRenderedBlocksAsync_ShouldFillHeaderPayloadWithDefaults()
    {
        // Arrange
        const string storeId = "ru";
        const string languageId = "ru_RU";
        const string key = "header";
        const string type = "header_default";
        const string variant = "default";

        var request = CreateRequest(
            storeId, 
            languageId,
            productCacheKey: string.Empty,
            isConstructor: false,
            templateType: "template_Info",
            (key, type, variant));
        
        var contentData = _mapper.MapToContentData(request);

        var expectedPayload = CreateDefaultBlockDataPayload(key);

        SetupBlockMocks(
            storeId, 
            languageId, 
            (key, type, variant, expectedPayload, "<h1>Test header</h1>"));

        // Act
        var result = await _sut.GetRenderedBlocksAsync(
            contentData,
            CancellationToken.None);

        // Assert
        var header = Assert.Single(result);

        Assert.Equal(key, header.Key);
        Assert.Equal(expectedPayload, header.Payload);
    }
    
    
    [Fact]
    public async Task GetRenderedBlocksAsync_ShouldFillFooterPayloadWithDefaults()
    {
        // Arrange
        const string storeId = "ru";
        const string languageId = "ru_RU";
        const string key = "footer";
        const string type = "footer_default";
        const string variant = "default";

        var request = CreateRequest(
            storeId, 
            languageId,
            productCacheKey: string.Empty,
            isConstructor: false,
            templateType: "template_Info",
            (key, type, variant));
        
        var contentData = _mapper.MapToContentData(request);

        var expectedPayload = CreateDefaultBlockDataPayload(key);
        
        SetupBlockMocks(
            storeId, 
            languageId, 
            (key, type, variant, expectedPayload, "<h1>Test footer</h1>"));

        // Act
        var result = await _sut.GetRenderedBlocksAsync(
            contentData,
            CancellationToken.None);

        // Assert
        var footer = Assert.Single(result);

        Assert.Equal(key, footer.Key);
        Assert.Equal(expectedPayload, footer.Payload);
    }
    
    private void SetupBlockMocks(
        string storeId,
        string languageId,
        params (string Key, string Type, string Variant,
            Dictionary<string, string> Payload, string Html)[] blocks)
    {
        _contentBlockRepository
            .Setup(x => x.GetBlocks(
                storeId,
                It.IsAny<List<BlockData>>()))
            .ReturnsAsync(blocks.Select(x =>
                CreateDefaultBlock(
                    storeId, languageId,
                    x.Key, x.Type, x.Variant,
                    html: x.Html)).ToList());

        _contentBlockRepository
            .Setup(x => x.GetBlocksDefaultData(
                storeId,
                languageId,
                It.IsAny<List<BlockData>>()))
            .ReturnsAsync(blocks.Select(x =>
                CreateDefaultBlockData(
                    storeId, languageId,
                    x.Key, x.Type, x.Variant,
                    x.Payload)).ToList());

        _contentBlockRepository
            .Setup(x => x.GetBlockWrappers())
            .ReturnsAsync([CreateDefaultWrapper()]);
    }
    
    private void SetupProductCache(
        string productCacheKey,
        int productCount = 6)
    {
        _productCache
            .Setup(x => x.TryGetProducts(
                productCacheKey,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenerateProducts(productCount).ToList());
    }

    /// <summary>
    /// Создаёт тестовый запрос на получение шаблона письма с заданными параметрами.
    /// </summary>
    /// <param name="storeId">Страна</param>
    /// <param name="languageId">Язык</param>
    /// <param name="productCacheKey">Ключ кэша</param>
    /// <param name="isConstructor">Указывает, используется ли режим конструктора.</param>
    /// <param name="templateType">Тип шаблона письма.</param>
    /// <param name="blocks">Ключ, тип и вариант блоков запроса.</param>
    /// <returns>Тестовый запрос с заполненными обязательными полями.</returns>
    private static GetEmailTemplateRequest CreateRequest(
        string storeId,
        string languageId,
        string productCacheKey = "",
        bool isConstructor = false,
        string templateType = "template_Info",
        params (string Key, string Type, string Variant)[] blocks) => new()
        {
            StoreId = storeId,
            LanguageId = languageId,
            ProductCacheKey = productCacheKey,
            IsConstructor = isConstructor,
            TemplateType = templateType,
            Blocks = blocks
                .Select(x => CreateBlock(x.Key, x.Type, x.Variant))
                .ToList()
        };
    
    /// <summary>
    /// Генератор продуктов для кеширования
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    private static IEnumerable<Product> GenerateProducts(int count = 6)
    {
        var random = new Random();

        for (var i = 0; i < count; i++)
        {
            yield return new Product
            {
                Link = $"https://example.com/product/{Guid.NewGuid():N}",
                Type = "CARE",
                Brand = $"BRAND_{random.Next(1, 100)}",
                Name = $"test product {Guid.NewGuid():N}",
                Description = $"Test description {random.Next(1000, 9999)}",
                Price = random.Next(100, 10000).ToString(),
                OldPrice = random.Next(10000, 20000).ToString(),
                Sku = $"SKU-{Guid.NewGuid():N}",
                Img = $"https://example.com/images/{Guid.NewGuid():N}.jpg"
            };
        }
    }

    private static DefaultBlock CreateDefaultBlock(
        string storeId,
        string languageId,
        string key,
        string type,
        string variant,
        Dictionary<string, string>? payload = null,
        string html = "",
        string css = "")
    {
        return new DefaultBlock
        {
            StoreId = storeId,
            Key = key,
            Type = type,
            Variant = variant,
            Payload = payload ?? new Dictionary<string, string>(),
            Localizations =
            [
                new Localization
                {
                    LanguageId = languageId,
                    Html = html,
                    Css = css
                }
            ]
        };
    }

    private static DefaultBlockData CreateDefaultBlockData(
        string storeId,
        string languageId,
        string key,
        string type,
        string variant,
        Dictionary<string, string> payload)
            {
                return new DefaultBlockData
                {
                    StoreId = storeId,
                    LanguageId = languageId,
                    Key = key,
                    Type = type,
                    Variant = variant,
                    Payload = payload
                };
            }

        private static BlockWrapper CreateDefaultWrapper(string html = "#{content}")
        {
            return new BlockWrapper
            {
                WrapperType = WrapperTypes.Default,
                WrapperHtml = html
            };
        }

        private static Dictionary<string, string> CreateDefaultBlockDataPayload(string key) => 
            key switch
        {
            "header" => new Dictionary<string, string>
                {
                    ["url"] = "https://goldapple.test.header.ru"
                },
            
            "footer" => new Dictionary<string, string>
                {
                    ["url"] = "https://goldapple.test.footer.ru"
                },

            "button" => new Dictionary<string, string>
                {
                    ["img_desktop"] = "https://email-images.mindbox.ru/708656/4764969/18.jpg",
                    ["img_mobile"] = "https://email-images.mindbox.ru/708656/4764969/23.jpg",
                    ["alt"] = "Перейти к покупкам из теста",
                    ["url"] = "https://goldapple.test.button.ru"
                },
            
            "text" => new Dictionary<string, string>
            {
                    ["cd_dates"] = "1 января из теста",
                    ["title"] = "Заголовок из теста",
                    ["font_family"] = "Helvetica, Arial, sans-serif",
                    ["text"] = "Lorem, ipsum dolor sit amet consectetur adipisicing elit. Accusam… из теста",
                    ["url"] = "https://goldapple.test.text.button.ru",
                    ["img_desktop"] = "https://email-images.mindbox.ru/Goldappleru/9ee910a7-511a-43b9-ab4a-2e…",
                    ["img_mobile"] = "https://email-images.mindbox.ru/Goldappleru/9ee910a7-511a-43b9-ab4a-2e…",
                    ["alt"] = "Сатып алуға өту из теста"
                },

            "banner" => new Dictionary<string, string>
            {
                    ["img_desktop"] = "https://email-images.mindbox.ru/Goldappleru/18275/416ddb12-30c7-45f4-b…",
                    ["img_mobile"] = "https://email-images.mindbox.ru/Goldappleru/18275/339ba462-3b07-44cd-a…",
                    ["alt"] = "alt_banner_ из_теста",
                    ["url"] = "https://goldapple.test.banner.ru"
                },
            
            _ => throw new ArgumentOutOfRangeException(nameof(key), key, null)
        };

        private static Block CreateBlock(
            string key,
            string type,
            string variant = "default") => new()
        {
            Key = key,
            Type = type,
            Variant = variant,
            Payload = new Dictionary<string, string>()
        };
        
}