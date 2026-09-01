using StockMapSvelte.Application.Abstractions;

namespace StockMapSvelte.Infrastructure.Cache;

public class CacheTagsProvider : ICacheTags
{
    private class PortfolioTags : ICacheTags.IPortfolioTags
    {
        public string TreemapData() => CacheTags.Portfolio.TreemapData;
    }
    
    public ICacheTags.IPortfolioTags Portfolio { get; } = new PortfolioTags();
}