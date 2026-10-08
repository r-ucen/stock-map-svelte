using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Exceptions.Portfolio;
using StockMapSvelte.Application.DTOs;
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
            .Select(s => new TreemapNodeDto
            {
                TickerSymbol = s.TickerSymbol,
                Sector = s.StockProfile.Sector ?? "Unknown",
                FullName = s.StockProfile.FullName ?? string.Empty,
                MarketCap = s.StockProfile.MarketCap ?? 0,
                RegularMarketChangePercent = s.StockProfile.RegularMarketChangePercent,
                RegularMarketPrice = s.StockProfile.RegularMarketPrice,
                PreMarketChangePercent = s.StockProfile.PreMarketChangePercent,
                PreMarketPrice = s.StockProfile.PreMarketPrice,
                PostMarketChangePercent = s.StockProfile.PostMarketChangePercent,
                PostMarketPrice = s.StockProfile.PostMarketPrice,
                MarketState = s.StockProfile.MarketState,
                Currency = s.StockProfile.Currency,
                Volume = s.StockProfile.Volume,
                DividendDate = s.StockProfile.DividendDate,
                ExDividendDate = s.StockProfile.ExDividendDate,
                DividendYield = s.StockProfile.DividendYield,
                EarningsDate = s.StockProfile.EarningsDate,
                Beta = s.StockProfile.Beta,
                Pe = s.StockProfile.Pe,
                ForwardPe = s.StockProfile.ForwardPe,
                ShortRatio = s.StockProfile.ShortRatio,
                AnalystRecommendationMean = s.StockProfile.AnalystRecommendationMean,
                AnalystRecommendationKey = s.StockProfile.AnalystRecommendationKey,
                ProfitMargins = s.StockProfile.ProfitMargins,
                EarningsQuarterlyGrowth = s.StockProfile.EarningsQuarterlyGrowth,
                TrailingEps = s.StockProfile.TrailingEps,
                ForwardEps = s.StockProfile.ForwardEps,
                PegRatio = s.StockProfile.PegRatio,
                OneYearChange = s.StockProfile.OneYearChange,
                TargetHighPrice = s.StockProfile.TargetHighPrice,
                TargetLowPrice = s.StockProfile.TargetLowPrice,
                TargetMeanPrice = s.StockProfile.TargetMeanPrice,
                TargetMedianPrice = s.StockProfile.TargetMedianPrice,
                TotalDebt = s.StockProfile.TotalDebt,
                FreeCashflow = s.StockProfile.FreeCashflow,
                EarningsGrowth = s.StockProfile.EarningsGrowth,
                RevenueGrowth = s.StockProfile.RevenueGrowth
            }).ToList();
        
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