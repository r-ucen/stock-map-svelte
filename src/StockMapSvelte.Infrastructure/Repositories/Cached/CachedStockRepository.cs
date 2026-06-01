using Microsoft.Extensions.Caching.Hybrid;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.DTOs;
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
    
    public async Task<StockDto?> GetStockViewModelByIdAsync(Guid stockId)
        => await _decorated.GetStockViewModelByIdAsync(stockId);
    
    public async Task<IReadOnlyList<StockDto>?> GetAllStocksAsync()
        =>  await _decorated.GetAllStocksAsync();
    
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
    
    // modifying

    public Task<int> CreateStockAsync(Stock stock)
        => _decorated.CreateStockAsync(stock);

    public Task<int> DeleteStockAsync(Guid stockId)
        => _decorated.DeleteStockAsync(stockId);

    public Task<int> EditStockAsync(Guid stockId, string ticker)
        => _decorated.EditStockAsync(stockId, ticker);
    
    // helpers
    
    public Task<bool> StockExistsAsync(string ticker)
        => _decorated.StockExistsAsync(ticker);

    public Task<bool> StockExistsAsync(Guid stockId)
        => _decorated.StockExistsAsync(stockId);
}