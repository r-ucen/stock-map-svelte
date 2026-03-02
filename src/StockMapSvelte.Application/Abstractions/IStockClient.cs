using StockMapSvelte.Domain.Entities;

namespace StockMapSvelte.Application.Abstractions;

public interface IStockClient
{
    public Task<bool> TickerExists(string ticker);
    Task<IReadOnlyList<StockProfile>> GetStockProfilesAsync();
}