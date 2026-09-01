using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.UseCases.StockUseCases.Queries;

namespace StockMapSvelte.Application.UseCases.StockUseCases.Handlers;

public class GetPossibleToAddStocksHandler
{
    private readonly IUnitOfWork  _unitOfWork;
    private readonly ICacheService _cache;
    
    public GetPossibleToAddStocksHandler(IUnitOfWork unitOfWork, ICacheService cache)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
    }
    
    public async Task<IReadOnlyList<StockDto>> Handle(GetPossibleToAddStocksQuery query, CancellationToken cancellationToken)
    {
        var filter = query.Filter.Trim().ToUpperInvariant();
        
        var upperTickersInPortfolio = query.StocksInPortfolio
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => t.Trim().ToUpperInvariant())
            .ToList();

        var portfolioStocksSortedString = string.Join(",", upperTickersInPortfolio.OrderBy(t => t));
        var cacheKey = _cache.Keys.Stock.PossibleToAdd(filter, portfolioStocksSortedString);

        return await _cache.GetOrCreateAsync(
            cacheKey,
            async ct => await _unitOfWork.Stocks.GetPossibleToAddAsync(filter, upperTickersInPortfolio, ct),
            expiration: TimeSpan.FromMinutes(3),
            localCacheExpiration: TimeSpan.FromMinutes(1),
            cancellationToken: cancellationToken);
    }
}