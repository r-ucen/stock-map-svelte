using Microsoft.EntityFrameworkCore;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Domain.Entities;
using StockMapSvelte.Infrastructure.Database;

namespace StockMapSvelte.Infrastructure.Repositories;

public class StockProfileRepository : IStockProfileRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public StockProfileRepository(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }
    
    public async Task<IReadOnlyList<StockStockProfileDto>> GetAllStockProfilesAsync(CancellationToken cancellationToken)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync(cancellationToken);
            
            return await dbContext.Stocks
                .AsNoTracking()
                .Include(s => s.StockProfile)
                .OrderBy(s => s.TickerSymbol)
                .Select(s => new StockStockProfileDto
                {
                    TickerSymbol = s.TickerSymbol,
                    Date = s.StockProfile.Date,
                    FullName = s.StockProfile.FullName,
                    Sector = s.StockProfile.Sector,
                    Currency = s.StockProfile.Currency,
                    RegularMarketChangePercent = s.StockProfile.RegularMarketChangePercent,
                    RegularMarketPrice = s.StockProfile.RegularMarketPrice,
                    EarningsDate = s.StockProfile.EarningsDate,
                    DividendDate = s.StockProfile.DividendDate,
                    ExDividendDate = s.StockProfile.ExDividendDate,
                    DividendYield = s.StockProfile.DividendYield,
                    Beta = s.StockProfile.Beta,
                    Pe = s.StockProfile.Pe,
                    ForwardPe = s.StockProfile.ForwardPe,
                    ShortRatio = s.StockProfile.ShortRatio,
                    AnalystRecommendationMean = s.StockProfile.AnalystRecommendationMean,
                    AnalystRecommendationKey = s.StockProfile.AnalystRecommendationKey,
                    Volume = s.StockProfile.Volume,
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
                })
                .ToListAsync(cancellationToken);
        }
    
    public async Task<int> SaveStockProfilesAsync(IEnumerable<StockProfile> profiles, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            
        var profileList = profiles
            .GroupBy(p => p.StockId)
            .Select(g => g.OrderByDescending(p => p.Date).First())
            .ToList();

        if (profileList.Count == 0)
        {
            return 0;
        }

        var stockIds = profileList.Select(p => p.StockId).Distinct().ToList();

        // existing profiles for the profiles being saved
        var existingProfiles = await context.StockProfiles
            .Where(sp => stockIds.Contains(sp.StockId))
            .ToListAsync(cancellationToken);

        var existingByStockId = existingProfiles.ToDictionary(sp => sp.StockId);

        foreach (var incoming in profileList)
        {
            if (existingByStockId.TryGetValue(incoming.StockId, out var existing))
            {
                incoming.Id = existing.Id;
                incoming.StockId = existing.StockId;

                context.Entry(existing).CurrentValues.SetValues(incoming);
            }
            else
            {
                if (incoming.Id == Guid.Empty)
                {
                    incoming.Id = Guid.NewGuid();
                }

                context.StockProfiles.Add(incoming);
            }
        }

        return await context.SaveChangesAsync(cancellationToken);
    }
}