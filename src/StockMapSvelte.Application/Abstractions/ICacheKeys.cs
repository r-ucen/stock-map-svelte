namespace StockMapSvelte.Application.Abstractions;

public interface ICacheKeys
{
    string TreemapData(Guid portfolioId);
    string PossibleToAdd(string filter, string portfolioStocks);
    string Banned(string userId);
}