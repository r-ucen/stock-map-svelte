using Microsoft.EntityFrameworkCore;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.DTOs.Common;
using StockMapSvelte.Domain.Entities;
using StockMapSvelte.Infrastructure.Database;
using StockMapSvelte.Infrastructure.Extensions;
using StockMapSvelte.Infrastructure.Extensions.Portfolio;

namespace StockMapSvelte.Infrastructure.Repositories;

public class PortfolioRepository : Repository<Portfolio>, IPortfolioRepository
{
    public PortfolioRepository(ApplicationDbContext dbContext) : base(dbContext) { }
    
    public async Task<IReadOnlyList<Portfolio>> GetAllPortfoliosAsync()
    {
        return await DbSet
            .Include(p => p.Stocks)
            .ToListAsync();
    }

    public async Task<bool> PortfolioNameExistsAsync(string userId, string portfolioName, CancellationToken cancellationToken)
    {
        return await DbSet
            .AnyAsync(p => p.Name == portfolioName && p.UserId == userId, cancellationToken);
    }

    public async Task<int> CreatePortfolioAsync(Portfolio portfolio, IList<string> tickerSymbols, CancellationToken cancellationToken)
    {
        var stocks =  await Context.Stocks
            .Where(s => tickerSymbols.Contains(s.TickerSymbol))
            .ToListAsync(cancellationToken);

        portfolio.AssignStocks(stocks);

        await Context.Portfolios.AddAsync(portfolio, cancellationToken);
        return await Context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Portfolio>?> GetPortfoliosByUserIdAsync(string userId, CancellationToken cancellationToken)
    {
        return await DbSet
            .Where(p => p.UserId == userId)
            .Include(p => p.Stocks.Where(s => s.IsInitialized))
            .ToListAsync(cancellationToken);
    }
    
    public async Task<bool> PortfolioNameExistsAsync(string userId, Guid portfolioId, string portfolioName, CancellationToken cancellationToken)
    {
        return await DbSet
            .AnyAsync(p => p.UserId == userId &&  p.Id != portfolioId && p.Name == portfolioName, cancellationToken);
    }

    public async Task<int> EditPortfolioAsync(Guid portfolioId, string portfolioName, IList<string> tickerSymbols, CancellationToken cancellationToken)
    {
        var portfolioToEdit = await DbSet
            .Include(p => p.Stocks)
            .FirstOrDefaultAsync(p => p.Id == portfolioId, cancellationToken);

        if (portfolioToEdit == null) { return 0; }
        
        var tickers = tickerSymbols
            .Where(ts => !string.IsNullOrWhiteSpace(ts))
            .Select(ts => ts.Trim())
            .ToList();
        
        var stocks = await Context.Stocks
            .Where(s => tickers.Contains(s.TickerSymbol))
            .ToListAsync(cancellationToken);
        
        portfolioToEdit.UpdateStocks(stocks);
        portfolioToEdit.UpdateName(portfolioName);
        
        Context.Portfolios.Update(portfolioToEdit);
        return await Context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task<int> DeletePortfolioAsync(Guid portfolioId, CancellationToken cancellationToken)
    {
        return await DbSet
            .Where(p => p.Id == portfolioId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<Portfolio?> GetPortfolioByIdAsync(Guid portfolioId, CancellationToken cancellationToken)
    {
        return await DbSet
            .Include(p => p.Stocks)
            .FirstOrDefaultAsync(p => p.Id == portfolioId, cancellationToken);
    }
    
    public async Task<PortfolioDto?> GetPortfolioByIdForUserAsync(string userId, Guid portfolioId, CancellationToken cancellationToken)
    {
        var userSetting = await Context.UserSettings
            .FirstOrDefaultAsync(us => us.UserId == userId, cancellationToken);

        var defaultPortfolioId = userSetting?.DefaultPortfolioId;

        return await DbSet
            .Where(p => p.Id == portfolioId && p.UserId == userId)
            .Include(p => p.Stocks.Where(s => s.IsInitialized))
            .Select(p => new PortfolioDto
            {
                PortfolioId = p.Id,
                UserId = p.UserId,
                PortfolioName = p.Name,
                IsDefault = p.Id == defaultPortfolioId,
                TickerSymbols = p.Stocks.Select(s => s.TickerSymbol).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> GetPortfolioCountByUserIdAsync(string userId, CancellationToken cancellationToken)
    {
        return await DbSet
            .CountAsync(p => p.UserId == userId, cancellationToken);
    }
    
    // REFACTORED

    public async Task<Portfolio?> GetByIdAsync(string userId, Guid portfolioId, CancellationToken cancellationToken)
    {
        return await DbSet
            .Where(p => p.Id == portfolioId && p.UserId == userId)
            .Include(p => p.Stocks.Where(s => s.IsInitialized))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> GetCountForUserAsync(string userId, CancellationToken cancellationToken)
    {
        return await DbSet
            .CountAsync(p => p.UserId == userId, cancellationToken);
    }

    // when creating a portfolio
    public async Task<bool> NameExistsAsync(string userId, string portfolioName, CancellationToken cancellationToken)
    {
        return await DbSet
            .AnyAsync(p => p.Name == portfolioName && p.UserId == userId, cancellationToken);
    }
    
    // when editing a portfolio
    public async Task<bool> NameExistsAsync(string userId, Guid portfolioId, string portfolioName, CancellationToken cancellationToken)
    {
        return await DbSet
            .AnyAsync(p => p.UserId == userId &&  p.Id != portfolioId && p.Name == portfolioName, cancellationToken);
    }
    
    public new async Task<Portfolio?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await DbSet
            .Include(p => p.Stocks)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }
    
    public async Task<Portfolio?> GetWithStockProfilesByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await DbSet
            .Include(p => p.Stocks.Where(s => s.IsInitialized))
            .ThenInclude(s => s.StockProfile)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }
    
    public async Task<PagedResponse<PortfolioDto>> GetAllAsync(QueryFilter filter, CancellationToken cancellationToken)
    {
        var pageNumber = Math.Max(1, filter.PageNumber);
        var pageSize = Math.Clamp(filter.PageSize, 1, 50);

        var query = DbSet
            .Include(p => p.Stocks)
            .AsNoTracking()
            .AsQueryable();

        query = query.ApplySearch(filter.Search, filter.SearchBy);
        
        var totalRecords = await query.CountAsync(cancellationToken);
        
        query = query.ApplySort(
            string.IsNullOrWhiteSpace(filter.SortBy) ? "Name" : filter.SortBy);
        
        var portfolios = await query
            .ApplyPagination(pageNumber, pageSize)
            .Select(p => new PortfolioDto
            {
                PortfolioId = p.Id,
                UserId = p.UserId,
                PortfolioName = p.Name,
                TickerSymbols = p.Stocks.Select(s => s.TickerSymbol).ToList()
                
            })
            .ToListAsync(cancellationToken);
        
        return new PagedResponse<PortfolioDto>
        {
            Data = portfolios,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalRecords,
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize)
        };
    }
    
    public async Task<IReadOnlyList<Portfolio>> GetForUserAsync(string userId, CancellationToken cancellationToken)
    {
        return await DbSet
            .Where(p => p.UserId == userId)
            .Include(p => p.Stocks.Where(s => s.IsInitialized))
            .ToListAsync(cancellationToken);
    }
    
    public async Task<Portfolio?> GetOwnershipInfoAsync(Guid portfolioId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(p => p.Id == portfolioId)
            .Select(p => new Portfolio { Id = p.Id, UserId = p.UserId, Name = p.Name })
            .FirstOrDefaultAsync(cancellationToken);
    }
}