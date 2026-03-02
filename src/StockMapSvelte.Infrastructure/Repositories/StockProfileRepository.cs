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
    
    public async Task<IReadOnlyList<StockStockProfileDto>> GetAllStockProfilesAsync()
        {
            await using var _dbContext = await _contextFactory.CreateDbContextAsync();
            
            return await _dbContext.Stocks
                .AsNoTracking()
                .Include(s => s.StockProfile)
                .OrderBy(s => s.TickerSymbol)
                .Select(s => new StockStockProfileDto
                {
                    TickerSymbol = s.TickerSymbol,
                    Date = s.StockProfile != null ? s.StockProfile.Date : (DateTimeOffset?)null,
                    FullName = s.StockProfile != null ? s.StockProfile.FullName : null,
                    Sector = s.StockProfile != null ? s.StockProfile.Sector : null,
                    Currency = s.StockProfile != null ? s.StockProfile.Currency : null,
                    RegularMarketChangePercent = s.StockProfile != null ? s.StockProfile.RegularMarketChangePercent : null,
                    RegularMarketPrice = s.StockProfile != null ? s.StockProfile.RegularMarketPrice : null,
                    EarningsDate = s.StockProfile != null ? s.StockProfile.EarningsDate : null,
                    DividendDate = s.StockProfile != null ? s.StockProfile.DividendDate : null,
                    ExDividendDate = s.StockProfile != null ? s.StockProfile.ExDividendDate : null,
                    DividendYield = s.StockProfile != null ? s.StockProfile.DividendYield : null,
                    Beta = s.StockProfile != null ? s.StockProfile.Beta : null,
                    Pe = s.StockProfile != null ? s.StockProfile.Pe : null,
                    ForwardPe = s.StockProfile != null ? s.StockProfile.ForwardPe : null,
                    ShortRatio = s.StockProfile != null ? s.StockProfile.ShortRatio : null,
                    AnalystRecommendationMean = s.StockProfile != null ? s.StockProfile.AnalystRecommendationMean : null,
                    AnalystRecommendationKey = s.StockProfile != null ? s.StockProfile.AnalystRecommendationKey : null,
                    Volume = s.StockProfile != null ? s.StockProfile.Volume : null
                })
                .ToListAsync();
        }
    
    public async Task<int> SaveStockProfilesAsync(IEnumerable<StockProfile> profiles)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
            
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
            .ToListAsync();

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

        return await context.SaveChangesAsync();
    }
}