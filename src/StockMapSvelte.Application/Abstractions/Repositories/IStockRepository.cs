using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.DTOs.Common;
using StockMapSvelte.Domain.Entities;

namespace StockMapSvelte.Application.Abstractions.Repositories;

public interface IStockRepository
{
    public Task<IList<string>> GetUninitializedStocks(IList<string> tickerSymbols, CancellationToken cancellationToken);
    Task<PagedResponse<StockDto>> GetAllStocksAsyncQueried(QueryFilter filter, CancellationToken cancellationToken);
    Task<bool> StockExistsAsync(string ticker, CancellationToken cancellationToken);
    Task<bool> StockExistsAsync(Guid stockId, CancellationToken cancellationToken);
    Task<int> CreateStockAsync(Stock stock);
    Task<int> DeleteStockAsync(Guid stockId, CancellationToken cancellationToken);
    Task<StockDto?> GetStockViewModelByIdAsync(Guid stockId, CancellationToken cancellationToken);
    Task<int> EditStockAsync(Guid stockId, string ticker, CancellationToken cancellationToken);
    Task<IReadOnlyList<StockDto>> GetPossibleToAddStocksAsync(string filter, IList<string> stocksInPortfolio, CancellationToken cancellationToken);
    Task<IReadOnlyList<Stock>> GetUninitializedStocksAsync(CancellationToken cancellationToken);
    Task<int> MarkStocksAsInitializedAsync(IEnumerable<Guid> stockIds, CancellationToken cancellationToken);
}