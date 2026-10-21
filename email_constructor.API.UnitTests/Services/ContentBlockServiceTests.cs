using email_constructor.Api.Extensions;
using email_constructor.Api.Model;
using email_constructor.Api.Model.Request;
using email_constructor.Application.Models;
using email_constructor.Application.Services;
using email_constructor.Cache;
using email_constructor.Domain.Enums;
using email_constructor.Domain.Exceptions;
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
    
    [Theory]
    [InlineData("ru", "ru_RU", "header", "header_default", "default")]
    [InlineData("ru", "ru_RU", "footer", "marketing", "with_offer")]
    [InlineData("ru", "ru_RU", "banner", "banner_default", "default")]
    [InlineData("ru", "ru_RU", "button", "button_default", "default")]
    [InlineData("ru", "ru_RU", "text", "text_default", "default")]
    [InlineData("ru", "ru_RU", "text", "text_default", "without_button")]
    [InlineData("ru", "ru_RU", "text", "text_default", "without_cd_dates")]
    [InlineData("ru", "ru_RU", "text", "text_default", "without_cd_dates_and_button")]
    public async Task GetRenderedBlocksAsync_ShouldFillPayloadWithDefaults(
        string storeId,
        string languageId,
        string key,
        string type,
        string variant)
    {
        var request = CreateRequest(
            storeId, 
            languageId,
            productCacheKey: string.Empty,
            isConstructor: false,
            templateType: "template_Info",
            (key, type, variant, null));
        
        var contentData = _mapper.MapToContentData(request);

        var expectedPayload = CreateDefaultBlockDataPayload(key, type, variant);
        
        SetupBlockMocks(
            storeId, 
            languageId, 
            (key, type, variant, expectedPayload, $"<h1>Test {key}</h1>"));
        
        var result = await _sut.GetRenderedBlocksAsync(
            contentData,
            CancellationToken.None);
        
        var block = Assert.Single(result);
        Assert.Equal(key, block.Key);
        Assert.Equal(variant, block.Variant);
        Assert.Equal(expectedPayload, block.Payload);
    }
    
    [Theory]
    [InlineData("ru", "ru_RU","banner", "banner_default", "default")]
    [InlineData("ru", "ru_RU","button", "button_default", "default")]
    [InlineData("ru", "ru_RU","text", "text_default", "default")]
    public async Task GetRenderedBlocksAsync_ShouldPreserveFullyFilledPayload(
        string storeId,
        string languageId,
        string key,
        string type,
        string variant)
    {
        var requestPayload = CreateDefaultBlockDataPayload(key, type, variant)
            .ToDictionary(x => x.Key, x => $"custom_{x.Value}");

        var expectedPayload = new Dictionary<string, string>(requestPayload);
        
        var request = CreateRequest(
            storeId,
            languageId,
            productCacheKey: string.Empty,
            isConstructor: false,
            templateType: "template_Info",
            (key, type, variant, requestPayload));

        var contentData = _mapper.MapToContentData(request);
        
        var defaultPayload = CreateDefaultBlockDataPayload(key, type, variant);

        SetupBlockMocks(
            storeId,
            languageId,
            (key, type, variant, defaultPayload, "<div>Test</div>"));

        var result = await _sut.GetRenderedBlocksAsync(
            contentData,
            CancellationToken.None);

        var block = Assert.Single(result);

        Assert.Equal(expectedPayload, block.Payload);
    }
    
    [Fact]
    public async Task GetRenderedBlocksAsync_ShouldFillMissingPayloadFieldsWithDefaults()
    {
        var storeId = "ru";
        var languageId = "ru_RU";
        var key = "text";
        var type = "text_default";
        var variant = "default";

        var requestPayload = new Dictionary<string, string>
        {
            ["title"] = "Заголовок из реквеста",
            ["url"] = "https://custom.test"
        };

        var defaultPayload = CreateDefaultBlockDataPayload(key, type, variant);

        var expectedPayload = new Dictionary<string, string>(requestPayload);

        foreach (var (payloadKey, value) in defaultPayload)
            expectedPayload.TryAdd(payloadKey, value);

        var request = CreateRequest(
            storeId,
            languageId,
            productCacheKey: string.Empty,
            isConstructor: false,
            templateType: "template_Info",
            (key, type, variant, requestPayload));

        var contentData = _mapper.MapToContentData(request);

        SetupBlockMocks(
            storeId,
            languageId,
            (key, type, variant, defaultPayload, "<div>Test</div>"));
        
        var result = await _sut.GetRenderedBlocksAsync(
            contentData,
            CancellationToken.None);
        
        var block = Assert.Single(result);

        Assert.Equal(expectedPayload.Count, block.Payload!.Count);

        foreach (var (payloadKey, value) in expectedPayload)
            Assert.Equal(value, block.Payload[payloadKey]);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetRenderedBlocksAsync_ShouldTreatEmptyRequestValueAsMissing(string emptyValue)
    {
        var storeId = "ru";
        var languageId = "ru_RU";
        var key = "banner";
        var type = "banner_default";
        var variant = "default";

        var requestPayload = new Dictionary<string, string>
        {
            ["url"] = "https://custom.test",
            ["alt"] = emptyValue
        };
        var defaultPayload = CreateDefaultBlockDataPayload(key, type, variant);

        var request = CreateRequest(
            storeId,
            languageId,
            productCacheKey: string.Empty,
            isConstructor: false,
            templateType: "template_Info",
            (key, type, variant, requestPayload));

        var contentData = _mapper.MapToContentData(request);

        SetupBlockMocks(storeId, languageId, (key, type, variant, defaultPayload, "<div>Test</div>"));

        var result = await _sut.GetRenderedBlocksAsync(contentData, CancellationToken.None);

        var block = Assert.Single(result);
        
        Assert.Equal("https://custom.test", block.Payload["url"]);
        Assert.Equal(defaultPayload["alt"], block.Payload["alt"]);
    }
    
    [Fact]
    public async Task GetRenderedBlocksAsync_ShouldSubstitutePayloadIntoHtml()
    {
        var storeId = "ru";
        var languageId = "ru_RU";
        var key = "banner";
        var type = "banner_default";
        var variant = "default";

        var requestPayload = new Dictionary<string, string>
        {
            ["url"] = "https://custom.test",
            ["alt"] = "Свой alt"
        };
        
        var defaultPayload = CreateDefaultBlockDataPayload(key, type, variant);

        var request = CreateRequest(
            storeId,
            languageId,
            productCacheKey: string.Empty,
            isConstructor: false,
            templateType: "template_Info",
            (key, type, variant, requestPayload));
        
        var contentData = _mapper.MapToContentData(request);

        const string html = "<a href=\"#{url}\"><img src=\"#{img_desktop}\" alt=\"#{alt}\"></a>";
        
        SetupBlockMocks(storeId, languageId, (key, type, variant, defaultPayload, html));

        var result = await _sut.GetRenderedBlocksAsync(contentData, CancellationToken.None);

        var block = Assert.Single(result);
        
        var expectedHtml = $"<a href=\"https://custom.test\"><img src=\"{defaultPayload["img_desktop"]}\" alt=\"Свой alt\"></a>";
        
        Assert.Equal(expectedHtml, block.Html);
    }
    
    [Fact]
    public async Task GetRenderedBlocksAsync_ShouldThrow_WhenProductTemplateHasNoProducts()
    {
        var request = CreateRequest(
            "ru", "ru_RU",
            productCacheKey: string.Empty,
            isConstructor: false,
            templateType: "template_S",
            ("header", "header_default", "default", null));
        
        var contentData = _mapper.MapToContentData(request);

        await Assert.ThrowsAsync<EmailConstructorExceptions.ProductsRequiredException>(
            () => _sut.GetRenderedBlocksAsync(contentData, CancellationToken.None));
        
        _contentBlockRepository.Verify(
            x => x.GetBlocks(It.IsAny<string>(), It.IsAny<List<BlockData>>()),
            Times.Never);
    }
    
    [Fact]
    public async Task GetRenderedBlocksAsync_ShouldNotThrow_WhenProductTemplateHasProducts()
    {
        var request = CreateRequest(
            "ru", "ru_RU",
            productCacheKey: "cacheKey",
            isConstructor: false,
            templateType: "template_S",
            ("header", "header_default", "default", null));
        
        var contentData = _mapper.MapToContentData(request);

        SetupProductCache("cacheKey");
        
        SetupBlockMocks(
            "ru", "ru_RU",
            ("header", "header_default", "default", CreateDefaultBlockDataPayload("header", "header_default", "default"), "<a></a>"));
        
        var result = await _sut.GetRenderedBlocksAsync(contentData, CancellationToken.None);
        
        Assert.Single(result);
        
        _productCache.Verify(
            x => x.TryGetProducts("cacheKey", It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Fact]
    public async Task GetRenderedBlocksAsync_ShouldNotThrow_WhenInfoTemplateHasNoCacheKey()
    {
        var request = CreateRequest(
            "ru", "ru_RU",
            productCacheKey: string.Empty,
            isConstructor: false,
            templateType: "template_info",
            ("header", "header_default", "default", null));
        
        var contentData = _mapper.MapToContentData(request);
        
        SetupBlockMocks(
            "ru", "ru_RU",
            ("header", "header_default", "default", CreateDefaultBlockDataPayload("header", "header_default", "default"), "<a></a>"));

        var result = await _sut.GetRenderedBlocksAsync(contentData, CancellationToken.None);
        
        Assert.Single(result);

        _contentBlockRepository.Verify(
            x => x.GetBlocks(It.IsAny<string>(), It.IsAny<List<BlockData>>()),
            Times.Once);
        
        _productCache.Verify(
            x => x.TryGetProducts(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
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
        params (string Key, string Type, string Variant, Dictionary<string, string>? Payload)[] blocks) => new()
        {
            StoreId = storeId,
            LanguageId = languageId,
            ProductCacheKey = productCacheKey,
            IsConstructor = isConstructor,
            TemplateType = templateType,
            Blocks = blocks
                .Select(x => CreateBlock(x.Key, x.Type, x.Variant, x.Payload))
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
        string html = "",
        string css = "")
    {
        return new DefaultBlock
        {
            StoreId = storeId,
            Key = key,
            Type = type,
            Variant = variant,
            Localizations =
            [
                new Localization
                {
                    LanguageId = languageId,
                    Html = html,
                    Css = css,
                    Payload = CreateDefaultBlockDataPayload( key, type, variant)
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

        private static Dictionary<string, string> CreateDefaultBlockDataPayload(string key, string type, string variant = "default") => 
            (key, type, variant) switch
        {
            ("header", "header_default", "default") => new Dictionary<string, string>
            {
                ["url"] = "https://goldapple.test.header.ru"
            },
            
            ("footer", "marketing", "with_offer")  => new Dictionary<string, string>
            {
                ["url"] = "https://goldapple.test.footer.ru"
            },

            ("button", "button_default", "default")  => new Dictionary<string, string>
            {
                ["img_desktop"] = "https://email-images.mindbox.ru/708656/4764969/18.jpg",
                ["img_mobile"] = "https://email-images.mindbox.ru/708656/4764969/23.jpg",
                ["alt"] = "Перейти к покупкам из теста",
                ["url"] = "https://goldapple.test.button.ru"
            },
            
            ("text", "text_default", "default")  => new Dictionary<string, string>
            {
                ["cd_dates"] = "1 января из теста default",
                ["title"] = "Заголовок из теста default",
                ["font_family"] = "Helvetica, Arial, sans-serif",
                ["text"] = "Lorem, ipsum dolor sit amet consectetur adipisicing elit. Accusam… из теста default",
                ["url"] = "https://goldapple.test.text.button.default.ru",
                ["img_desktop"] = "https://email-images.mindbox.ru/Goldappleru/9ee910a7-511a-43b9-ab4a-2e…",
                ["img_mobile"] = "https://email-images.mindbox.ru/Goldappleru/9ee910a7-511a-43b9-ab4a-2e…",
                ["alt"] = "Сатып алуға өту из теста default"
            },
            ("text", "text_default", "without_cd_dates")  => new Dictionary<string, string>
            {
                ["title"] = "Заголовок из теста without_cd_dates",
                ["font_family"] = "Helvetica, Arial, sans-serif",
                ["text"] = "Lorem, ipsum dolor sit amet consectetur adipisicing elit. Accusam… из теста without_cd_dates",
                ["url"] = "https://goldapple.test.text.button.without_cd_dates.ru",
                ["img_desktop"] = "https://email-images.mindbox.ru/Goldappleru/9ee910a7-511a-43b9-ab4a-2e…",
                ["img_mobile"] = "https://email-images.mindbox.ru/Goldappleru/9ee910a7-511a-43b9-ab4a-2e…",
                ["alt"] = "Сатып алуға өту из теста without_cd_dates"
            },
            ("text", "text_default", "without_button")  => new Dictionary<string, string>
            {
                ["cd_dates"] = "1 января из теста without_button",
                ["title"] = "Заголовок из теста without_button",
                ["font_family"] = "Helvetica, Arial, sans-serif",
                ["text"] = "Lorem, ipsum dolor sit amet consectetur adipisicing elit. Accusam… из теста without_button"
            },
            ("text", "text_default", "without_cd_dates_and_button")  => new Dictionary<string, string>
            {
                ["title"] = "Заголовок из теста without_cd_dates_and_button",
                ["font_family"] = "Helvetica, Arial, sans-serif",
                ["text"] = "Lorem, ipsum dolor sit amet consectetur adipisicing elit. Accusam… из теста without_cd_dates_and_button"
            },
            ("banner", "banner_default", "default")  => new Dictionary<string, string>
            {
                ["img_desktop"] = "https://email-images.mindbox.ru/Goldappleru/18275/416ddb12-30c7-45f4-b…",
                ["img_mobile"] = "https://email-images.mindbox.ru/Goldappleru/18275/339ba462-3b07-44cd-a…",
                ["alt"] = "alt_banner_ из_теста",
                ["url"] = "https://goldapple.test.banner.ru"
            },
            
            _ => new Dictionary<string, string>()
        };

        private static Block CreateBlock(
            string key,
            string type,
            string variant = "default",
            Dictionary<string, string>? payload = null) => new()
        {
            Key = key,
            Type = type,
            Variant = variant,
            Payload = payload ?? new Dictionary<string, string>()
        };
        
}