using Microsoft.EntityFrameworkCore;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.DTOs.Common;
using StockMapSvelte.Domain.Entities;
using StockMapSvelte.Infrastructure.Database;
using StockMapSvelte.Infrastructure.Extensions;
using StockMapSvelte.Infrastructure.Extensions.StockProfile;

namespace StockMapSvelte.Infrastructure.Repositories;

public class StockProfileRepository : Repository<StockProfile>, IStockProfileRepository
{
    public StockProfileRepository(ApplicationDbContext dbContext) : base(dbContext) { }
    
    public async Task<int> SaveAsync(IEnumerable<StockProfile> profiles, CancellationToken cancellationToken)
    {
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
        var existingProfiles = await DbSet
            .Where(sp => stockIds.Contains(sp.StockId))
            .ToListAsync(cancellationToken);

        var existingByStockId = existingProfiles.ToDictionary(sp => sp.StockId);

        foreach (var incoming in profileList)
        {
            if (existingByStockId.TryGetValue(incoming.StockId, out var existing))
            {
                incoming.Id = existing.Id;
                incoming.StockId = existing.StockId;

                DbSet.Entry(existing).CurrentValues.SetValues(incoming);
            }
            else
            {
                if (incoming.Id == Guid.Empty)
                {
                    incoming.Id = Guid.NewGuid();
                }

                DbSet.Add(incoming);
            }
        }

        return await Context.SaveChangesAsync(cancellationToken);
    }
    
    // REFACTORED
    
    public async Task<PagedResponse<StockStockProfileDto>> GetAllStockProfilesAsync(QueryFilter filter, CancellationToken cancellationToken)
    {
        var pageNumber = Math.Max(1, filter.PageNumber);
        var pageSize = Math.Clamp(filter.PageSize, 1, 50);

        var query = Context.Stocks
            .Include(s => s.StockProfile)
            .AsNoTracking()
            .AsQueryable();

        query = query.ApplySearch(filter.Search, filter.SearchBy);
        
        var totalRecords = await query.CountAsync(cancellationToken);
        
        query = query.ApplySort(
            string.IsNullOrWhiteSpace(filter.SortBy) ? "TickerSymbol" : filter.SortBy);

        var stockProfiles = await query
            .ApplyPagination(pageNumber, pageSize)
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
        
        return new PagedResponse<StockStockProfileDto>
        {
            Data = stockProfiles,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalRecords,
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize)
        };
    }
}