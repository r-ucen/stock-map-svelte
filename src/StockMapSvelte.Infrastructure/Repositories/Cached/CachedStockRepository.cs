using Microsoft.Extensions.Caching.Hybrid;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.DTOs.Common;
using StockMapSvelte.Domain.Entities;

namespace StockMapSvelte.Infrastructure.Repositories.Cached;

public class CachedStockRepository : IStockRepository
{
    private readonly IStockRepository _decorated;
    private readonly HybridCache _cache;
    
    public CachedStockRepository(IStockRepository decorated, HybridCache cache)
    {
        _decorated = decorated;
        _cache = cache;
    }
    
    // getting
    
    public async Task<StockDto?> GetStockViewModelByIdAsync(Guid stockId, CancellationToken cancellationToken)
        => await _decorated.GetStockViewModelByIdAsync(stockId, cancellationToken);

    public async Task<IList<string>> GetUninitializedStocks(IList<string> tickerSymbols, CancellationToken cancellationToken)
        => await _decorated.GetUninitializedStocks(tickerSymbols, cancellationToken);

    public async Task<PagedResponse<StockDto>> GetAllStocksAsyncQueried(QueryFilter filter, CancellationToken cancellationToken)
        => await _decorated.GetAllStocksAsyncQueried(filter, cancellationToken);

    public async Task<IReadOnlyList<StockDto>> GetPossibleToAddStocksAsync(string filter, IList<string> stocksInPortfolio, CancellationToken cancellationToken = default)
    {
        var portfolioStocksSortedString = string.Join(",", stocksInPortfolio
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => t.Trim().ToUpperInvariant())
            .OrderBy(t => t));
        
        var normalizedFilter = filter.Trim().ToUpperInvariant();
        var cacheKey = $"stocks:possible:f-{normalizedFilter}:p-{portfolioStocksSortedString}";
        
        var options = new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromMinutes(3),
            LocalCacheExpiration = TimeSpan.FromMinutes(1)
        };

        return await _cache.GetOrCreateAsync(
            cacheKey,
            async cancel => await _decorated.GetPossibleToAddStocksAsync(filter, stocksInPortfolio, cancel),
            options: options,
            cancellationToken: cancellationToken
        );
    }

    public Task<IReadOnlyList<Stock>> GetUninitializedStocksAsync(CancellationToken cancellationToken)
        => _decorated.GetUninitializedStocksAsync(cancellationToken);

    public Task<int> MarkStocksAsInitializedAsync(IEnumerable<Guid> stockIds, CancellationToken cancellationToken)
        => _decorated.MarkStocksAsInitializedAsync(stockIds, cancellationToken);

    // modifying

    public Task<int> CreateStockAsync(Stock stock)
        => _decorated.CreateStockAsync(stock);

    public Task<int> DeleteStockAsync(Guid stockId, CancellationToken cancellationToken)
        => _decorated.DeleteStockAsync(stockId, cancellationToken);

    public Task<int> EditStockAsync(Guid stockId, string ticker, CancellationToken cancellationToken)
        => _decorated.EditStockAsync(stockId, ticker, cancellationToken);
    
    // helpers
    
    public Task<bool> StockExistsAsync(string ticker, CancellationToken cancellationToken)
        => _decorated.StockExistsAsync(ticker, cancellationToken);

    public Task<bool> StockExistsAsync(Guid stockId, CancellationToken cancellationToken)
        => _decorated.StockExistsAsync(stockId, cancellationToken);
}