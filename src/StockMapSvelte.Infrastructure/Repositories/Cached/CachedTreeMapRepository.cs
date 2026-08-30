using Microsoft.Extensions.Caching.Hybrid;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Infrastructure.Cache;

namespace StockMapSvelte.Infrastructure.Repositories.Cached;

public class CachedTreeMapRepository : ITreeMapRepository
{
    private readonly ITreeMapRepository _decorated;
    private readonly HybridCache _cache;

    public CachedTreeMapRepository(ITreeMapRepository decorated, HybridCache cache)
    {
        _decorated = decorated;
        _cache = cache;
    }
    
    public async Task<TreemapDataDto> GetTreemapDataViewModelByIdAsync(Guid portfolioId, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.Portfolio.TreemapData(portfolioId);
        var tags = new[] { CacheTags.Portfolio.TreemapData };
        
        var options = new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromHours(1),
            LocalCacheExpiration = TimeSpan.FromHours(1)
        };

        return await _cache.GetOrCreateAsync(
            cacheKey,
            async cancel => await _decorated.GetTreemapDataViewModelByIdAsync(portfolioId, cancel),
            options: options,
            cancellationToken: cancellationToken,
            tags: tags
        );
    }
}