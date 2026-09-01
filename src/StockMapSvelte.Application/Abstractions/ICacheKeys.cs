namespace StockMapSvelte.Application.Abstractions;

public interface ICacheKeys
{
    IPortfolioKeys Portfolio { get; }
    IStockKeys Stock { get; }
    IUserKeys User { get; }
    
    public interface IPortfolioKeys
    {
        string TreemapData(Guid portfolioId);
    }

    public interface IStockKeys
    {
        string PossibleToAdd(string filter, string portfolioStocks);
    }

    public interface IUserKeys
    {
        string Banned(string userId);
    }
}