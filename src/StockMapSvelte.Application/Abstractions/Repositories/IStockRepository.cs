using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.DTOs.Common;
using StockMapSvelte.Domain.Entities;

namespace StockMapSvelte.Application.Abstractions.Repositories;

public interface IStockRepository : IRepository<Stock>
{
    Task<IReadOnlyList<Stock>> GetUninitializedAsync(CancellationToken cancellationToken);
    Task<int> MarkAsInitializedAsync(IEnumerable<Guid> stockIds, CancellationToken cancellationToken);
    Task<PagedResponse<StockDto>> GetAllAsync(QueryFilter filter, CancellationToken cancellationToken);
    public Task<IReadOnlyList<string>> GetMissingTickerSymbolsAsync(IList<string> tickerSymbols, CancellationToken cancellationToken);
    public Task<IList<string>> GetUninitializedTickerSymbolsAsync(IList<string> tickerSymbols, CancellationToken cancellationToken);
    public Task<bool> ExistsAsync(string ticker, CancellationToken cancellationToken);
    public Task<IReadOnlyList<StockDto>> GetPossibleToAddAsync(string filter, IList<string> stocksInPortfolio, CancellationToken cancellationToken);
}