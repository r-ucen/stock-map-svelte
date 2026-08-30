using Microsoft.Extensions.Caching.Hybrid;
using StockMapSvelte.Application.Abstractions;

namespace StockMapSvelte.Infrastructure.Cache;

public class HybridCacheService : ICacheService
{
    private readonly HybridCache _cache;
    
    public HybridCacheService(HybridCache cache)
    {
        _cache = cache;
    }
    
    public async Task<T> GetOrCreateAsync<T>(string key, Func<CancellationToken, Task<T>> factory, TimeSpan? expiration = null, TimeSpan? localCacheExpiration = null,
        IEnumerable<string>? tags = null, CancellationToken cancellationToken = default)
    {
        var options = new HybridCacheEntryOptions
        {
            Expiration = expiration,
            LocalCacheExpiration = localCacheExpiration
        };

        return await _cache.GetOrCreateAsync(
            key: key,
            factory: async cancel => await factory(cancel),
            options: options,
            cancellationToken: cancellationToken
        );
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, TimeSpan? localCacheExpiration = null,
        IEnumerable<string>? tags = null, CancellationToken cancellationToken = default)
    {
        var options = new HybridCacheEntryOptions
        {
            Expiration = expiration,
            LocalCacheExpiration = localCacheExpiration
        };

        await _cache.SetAsync(
            key: key,
            options: options,
            value: value,
            tags: tags,
            cancellationToken: cancellationToken
        );
    }

    public async Task RemoveByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync(key, cancellationToken);
    }

    public async Task RemoveByTagAsync(string tag, CancellationToken cancellationToken = default)
    {
        await _cache.RemoveByTagAsync(tag, cancellationToken);
    }
}