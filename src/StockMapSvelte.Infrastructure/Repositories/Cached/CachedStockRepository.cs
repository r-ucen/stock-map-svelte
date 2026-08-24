using System.Linq.Expressions;
using Microsoft.Extensions.Caching.Hybrid;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.DTOs.Common;
using StockMapSvelte.Domain.Entities;
using StockMapSvelte.Infrastructure.Repositories.Cached.CacheManagement;

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

    public Task<IList<string>> GetMissingStocksAsync(IList<string> tickerSymbols, CancellationToken cancellationToken)
        => _decorated.GetMissingStocksAsync(tickerSymbols, cancellationToken);

    public async Task<IList<string>> GetUninitializedStocks(IList<string> tickerSymbols, CancellationToken cancellationToken)
        => await _decorated.GetUninitializedStocks(tickerSymbols, cancellationToken);

    public async Task<PagedResponse<StockDto>> GetAllAsync(QueryFilter filter, CancellationToken cancellationToken)
        => await _decorated.GetAllAsync(filter, cancellationToken);

    public async Task<IReadOnlyList<StockDto>> GetPossibleToAddStocksAsync(string filter, IList<string> stocksInPortfolio, CancellationToken cancellationToken = default)
    {
        var portfolioStocksSortedString = string.Join(",", stocksInPortfolio
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => t.Trim().ToUpperInvariant())
            .OrderBy(t => t));
        
        var normalizedFilter = filter.Trim().ToUpperInvariant();
        var cacheKey = CacheKeys.Stock.PossibleToAdd(normalizedFilter, portfolioStocksSortedString);
        
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

    public Task<Stock?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _decorated.GetByIdAsync(id, cancellationToken);

    public Task<IReadOnlyList<Stock>> GetAllAsync(CancellationToken cancellationToken = default)
        => _decorated.GetAllAsync(cancellationToken);

    public Task<IReadOnlyList<Stock>> FindAsync(Expression<Func<Stock, bool>> predicate, CancellationToken cancellationToken = default)
        => _decorated.FindAsync(predicate, cancellationToken);

    public Task<Stock> AddAsync(Stock entity, CancellationToken cancellationToken = default)
        => _decorated.AddAsync(entity, cancellationToken);

    public void Update(Stock entity)
        => _decorated.Update(entity);

    public void Remove(Stock entity)
        => _decorated.Remove(entity);

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        => _decorated.ExistsAsync(id, cancellationToken);
    
    // REFACTORED
    
    public Task<IReadOnlyList<Stock>> FindTrackedAsync(Expression<Func<Stock, bool>> predicate, CancellationToken cancellationToken = default)
        => _decorated.FindTrackedAsync(predicate, cancellationToken);
    
    public Task<IReadOnlyList<string>> GetMissingTickerSymbolsAsync(IList<string> tickerSymbols, CancellationToken cancellationToken)
        => _decorated.GetMissingTickerSymbolsAsync(tickerSymbols, cancellationToken);

    public Task<IList<string>> GetUninitializedTickerSymbolsAsync(IList<string> tickerSymbols, CancellationToken cancellationToken)
        => _decorated.GetUninitializedTickerSymbolsAsync(tickerSymbols, cancellationToken);

    public Task<bool> ExistsAsync(string ticker, CancellationToken cancellationToken)
        => _decorated.ExistsAsync(ticker, cancellationToken);
}