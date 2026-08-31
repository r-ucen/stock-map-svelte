using StockMapSvelte.Application.Abstractions;

namespace StockMapSvelte.Infrastructure.Cache;

public class CacheTagsProvider : ICacheTags
{
    public string TreemapData => CacheTags.Portfolio.TreemapData;
}