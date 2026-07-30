using Microsoft.Extensions.Caching.Hybrid;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.DTOs.Common;
using StockMapSvelte.Domain.Entities;
using StockMapSvelte.Infrastructure.Repositories.Cached.CacheManagement;

namespace StockMapSvelte.Infrastructure.Repositories.Cached;

public class CachedPortfolioRepository : IPortfolioRepository
{
    private readonly IPortfolioRepository _decorated;
    private readonly HybridCache _cache;

    public CachedPortfolioRepository(IPortfolioRepository decorated, HybridCache cache)
    {
        _decorated = decorated;
        _cache = cache;
    }
    
    // getting
    
    public Task<PagedResponse<PortfolioStockDto>> GetAllPortfolioStockViewModelsAsync(QueryFilter filter, CancellationToken cancellationToken)
    => _decorated.GetAllPortfolioStockViewModelsAsync(filter, cancellationToken);

    public Task<int> CreatePortfolioAsync(Portfolio portfolio, IList<string> tickerSymbols, CancellationToken cancellationToken)
    => _decorated.CreatePortfolioAsync(portfolio, tickerSymbols, cancellationToken);

    public Task<IReadOnlyList<Portfolio>?> GetPortfoliosByUserIdAsync(string userId, CancellationToken cancellationToken)
    => _decorated.GetPortfoliosByUserIdAsync(userId, cancellationToken);
    
    public Task<Portfolio?> GetPortfolioByIdAsync(Guid portfolioId, CancellationToken cancellationToken)
        => _decorated.GetPortfolioByIdAsync(portfolioId, cancellationToken);
    
    public Task<PortfolioStockDto?> GetPortfolioByIdForUserAsync(string userId, Guid portfolioId, CancellationToken cancellationToken)
        => _decorated.GetPortfolioByIdForUserAsync(userId, portfolioId, cancellationToken);
    
    public Task<IReadOnlyList<Portfolio>> GetAllPortfoliosAsync()
        => _decorated.GetAllPortfoliosAsync();
    
    public Task<int> GetPortfolioCountByUserIdAsync(string userId, CancellationToken cancellationToken)
        => _decorated.GetPortfolioCountByUserIdAsync(userId, cancellationToken);
    
    // helpers
    
    public Task<bool> PortfolioNameExistsAsync(string userId, string portfolioName, CancellationToken cancellationToken)
        => _decorated.PortfolioNameExistsAsync(userId, portfolioName, cancellationToken);

    public Task<bool> PortfolioNameExistsAsync(string userId, Guid portfolioId, string portfolioName,
        CancellationToken cancellationToken)
    => _decorated.PortfolioNameExistsAsync(userId, portfolioId, portfolioName, cancellationToken);
    
    // modifying

    public async Task<int> EditPortfolioAsync(Guid portfolioId, string portfolioName, IList<string> tickerSymbols,
        CancellationToken cancellationToken)
    {
        var result = await _decorated.EditPortfolioAsync(portfolioId, portfolioName, tickerSymbols, cancellationToken);
        
        await _cache.RemoveAsync(CacheKeys.Portfolio.TreemapData(portfolioId), cancellationToken);
        
        return result;
    }

    public Task<int> DeletePortfolioAsync(Guid portfolioId, CancellationToken cancellationToken)
    => _decorated.DeletePortfolioAsync(portfolioId, cancellationToken);
}