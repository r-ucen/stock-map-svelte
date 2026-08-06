namespace StockMapSvelte.Infrastructure.Repositories.Cached.CacheManagement;

public class CacheKeys
{
    public static class Portfolio
    {
        public static string TreemapData(Guid portfolioId) => $"treemap-data:portfolio:{portfolioId}";
    }

    public static class Stock
    {
        public static string PossibleToAdd(string filter, string portfolioStocks) => $"stocks:possible:f-{filter}:p-{portfolioStocks}";
    }

    public static class User
    {
        public static string Banned(string userId) => $"user:banned:{userId}";
    }
}