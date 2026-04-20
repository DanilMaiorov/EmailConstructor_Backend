using email_constructor.Domain.Model;

namespace email_constructor.Cache;

/// <summary>
/// Кэш хранящий продукты после загрузки эксель файла
/// </summary>
public interface IProductCache
{
    /// <summary>
    /// Установить продукты в кэш.
    /// </summary>
    /// <param name="products">Коллекция Products</param>
    /// <param name="cancellationToken"></param>
    Task<string> SetCacheProducts(IEnumerable<Product> products, CancellationToken cancellationToken);
    
    /// <summary>
    /// Получить продукты из кэша (если время кэширование не истекло).
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>IReadOnlyDictionary с продуктами из кэша.</returns>
    Task<IReadOnlyList<Product>> TryGetProducts(string cacheKey, CancellationToken cancellationToken);
}