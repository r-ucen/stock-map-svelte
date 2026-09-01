using StockMapSvelte.Application.Abstractions;

namespace StockMapSvelte.Infrastructure.Cache;

public class CacheKeyProvider : ICacheKeys
{
    private class PortfolioKeys : ICacheKeys.IPortfolioKeys
    {
        public string TreemapData(Guid portfolioId) => CacheKeys.Portfolio.TreemapData(portfolioId);
    }

    private class StockKeys : ICacheKeys.IStockKeys
    {
        public string PossibleToAdd(string filter, string portfolioStocks) => CacheKeys.Stock.PossibleToAdd(filter, portfolioStocks);
    }

    private class UserKeys : ICacheKeys.IUserKeys
    {
        public string Banned(string userId) => CacheKeys.User.Banned(userId);
    }

    public ICacheKeys.IPortfolioKeys Portfolio { get; } = new PortfolioKeys();
    public ICacheKeys.IStockKeys Stock { get; } = new StockKeys();
    public ICacheKeys.IUserKeys User { get; } = new UserKeys();
}