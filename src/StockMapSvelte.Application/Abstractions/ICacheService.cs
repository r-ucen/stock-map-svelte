namespace StockMapSvelte.Application.Abstractions;

public interface ICacheService
{
    ICacheKeys Keys { get; }
    
    Task<T> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan? expiration = null,
        TimeSpan? localCacheExpiration = null,
        IEnumerable<string>? tags = null,
        CancellationToken cancellationToken = default);

    Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        TimeSpan? localCacheExpiration = null,
        IEnumerable<string>? tags = null,
        CancellationToken cancellationToken = default);
    
    Task RemoveByKeyAsync(string key, CancellationToken cancellationToken = default);
    Task RemoveByTagAsync(string tag, CancellationToken cancellationToken = default);
}