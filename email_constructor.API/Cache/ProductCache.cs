using email_constructor.Domain.Model;
using email_constructor.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace email_constructor.Cache;

public class ProductCache : IProductCache
{
    private readonly ProductCacheOptions _productCacheOptions;
    private readonly IMemoryCache _memoryCache;
    
    /// <summary>
    /// Конструктор <see cref="ProductCache" />
    /// </summary>
    public ProductCache(
        IMemoryCache memoryCache, 
        IOptions<ProductCacheOptions> productCacheOptions)
    {
        _memoryCache = memoryCache;
        _productCacheOptions = productCacheOptions.Value;
    }

    public async Task<string> SetCacheProducts(IEnumerable<Product> products, CancellationToken cancellationToken)
    {
        var cacheKey = $"excel_products_{Guid.NewGuid()}";
        
        _memoryCache.Set(cacheKey, products.ToList(), _productCacheOptions.ProductsCacheLifeInMinutes);
        
        return await Task.FromResult(cacheKey);
    }

    public Task<IReadOnlyList<Product>> TryGetProducts(string cacheKey, CancellationToken cancellationToken)
    {
        if (!_memoryCache.TryGetValue(cacheKey, out IReadOnlyList<Product>? products))
            throw new KeyNotFoundException($"Данные для ключа '{cacheKey}' не найдены в кэше. Возможно, сессия истекла.");
        
        if (products is null || products.Count == 0)
            throw new NullReferenceException($"Коллекция продуктов по ключу'{cacheKey}' пустая или не найдена.");
        
        var result = products as List<Product> ?? products.ToList();
        
        return Task.FromResult<IReadOnlyList<Product>>(result);
    }
}