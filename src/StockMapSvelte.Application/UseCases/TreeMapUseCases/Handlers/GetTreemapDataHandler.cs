using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Exceptions.Portfolio;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.Extensions.Stock;
using StockMapSvelte.Application.UseCases.TreeMapUseCases.Queries;

namespace StockMapSvelte.Application.UseCases.TreeMapUseCases.Handlers;

public class GetTreemapDataHandler
{
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStockProfileUpdateTracker _stockProfileUpdateTracker;
    
    public GetTreemapDataHandler(
        IUserContext userContext,
        IUnitOfWork unitOfWork,
        IStockProfileUpdateTracker stockProfileUpdateTracker)
    {
        _userContext = userContext;
        _unitOfWork = unitOfWork;
        _stockProfileUpdateTracker = stockProfileUpdateTracker;
    }
    
    public async Task<TreemapDataDto> Handle(GetTreemapDataQuery query, CancellationToken cancellationToken)
    {
        var currentUserId = await _userContext.GetCurrentUserIdAsync();
        
        var portfolioOwnership = await _unitOfWork.Portfolios.GetOwnershipInfoAsync(query.PortfolioId, cancellationToken);
        if (portfolioOwnership == null) { throw new PortfolioNotFoundException("Portfolio not found."); }
            
        portfolioOwnership.ValidateOwnership(currentUserId);

        var portfolio = await _unitOfWork.Portfolios.GetWithStockProfilesByIdAsync(query.PortfolioId, cancellationToken);
        if (portfolio == null) { throw new PortfolioNotFoundException("Portfolio not found."); }
                    
        var nodes = portfolio.Stocks
            .Select(s => s.ToTreemapNodeDto()).ToList();
        
        var sectors = nodes
            .GroupBy(n => n.Sector)
            .Select(g => new TreemapSectorDto
            {
                SectorName = g.Key,
                TotalMarketCap = g.Sum(n => n.MarketCap),
                Stocks = g.ToList()
            })
            .ToList();
        
        return new TreemapDataDto
        {
            Sectors = sectors,
            TotalMarketCap = sectors.Sum(s => s.TotalMarketCap),
            LastUpdated = _stockProfileUpdateTracker.LastUpdated
        };
    }
}