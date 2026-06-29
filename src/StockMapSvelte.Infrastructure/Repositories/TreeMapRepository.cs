using Microsoft.EntityFrameworkCore;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.Exceptions.Portfolio;
using StockMapSvelte.Infrastructure.Database;

namespace StockMapSvelte.Infrastructure.Repositories;

public class TreeMapRepository : ITreeMapRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public TreeMapRepository(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }
    
    public async Task<TreemapDataDto> GetTreemapDataViewModelByIdAsync(Guid portfolioId, CancellationToken cancellationToken)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            
            var portfolio = await context.Portfolios
                .Include(p => p.Stocks.Where(s => s.IsInitialized))
                    .ThenInclude(s => s.StockProfile)
                .FirstOrDefaultAsync(p => p.Id == portfolioId, cancellationToken);

            if (portfolio == null) { throw new PortfolioNotFoundException("Portfolio with the specified ID was not found."); }
            
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
                TotalMarketCap = sectors.Sum(s => s.TotalMarketCap)
            };
        }
}