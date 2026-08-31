using StockMapSvelte.Application.Abstractions;

namespace StockMapSvelte.Infrastructure.Cache;

public class CacheKeyProvider : ICacheKeys
{
    public string TreemapData(Guid portfolioId) => CacheKeys.Portfolio.TreemapData(portfolioId);
    public string PossibleToAdd(string filter, string portfolioStocks) => CacheKeys.Stock.PossibleToAdd(filter, portfolioStocks);
    public string Banned(string userId) => CacheKeys.User.Banned(userId);
}