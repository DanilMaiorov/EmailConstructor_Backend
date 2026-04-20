namespace email_constructor.Options;

/// <summary>
/// Настройки кэша для <see cref="ProductCacheCache"/>
/// </summary>
public sealed record ProductCacheOptions
{
    /// <summary>
    /// Время жизни кеша для глобальных переменных.
    /// </summary>
    public TimeSpan ProductsCacheLifeInMinutes { get; init; }
}
