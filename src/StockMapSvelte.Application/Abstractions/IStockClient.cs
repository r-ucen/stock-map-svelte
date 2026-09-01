using StockMapSvelte.Domain.Entities;

namespace StockMapSvelte.Application.Abstractions;

public interface IStockClient
{
    public Task<string> GetYahooTickerSymbol(string isin);
    public Task<bool> TickerSymbolExists(string ticker);
    Task<IReadOnlyList<StockProfile>> GetStockProfilesAsync(CancellationToken cancellationToken);
}