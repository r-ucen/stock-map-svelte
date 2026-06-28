using Microsoft.EntityFrameworkCore;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.DTOs.Common;
using StockMapSvelte.Application.Exceptions.Stock;
using StockMapSvelte.Domain.Entities;
using StockMapSvelte.Infrastructure.Database;
using StockMapSvelte.Infrastructure.Extensions;
using StockMapSvelte.Infrastructure.Extensions.Portfolio;

namespace StockMapSvelte.Infrastructure.Repositories;

public class PortfolioRepository : IPortfolioRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public PortfolioRepository(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<PagedResponse<PortfolioStockDto>> GetAllPortfolioStockViewModelsAsync(QueryFilter filter, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        
        var pageNumber = Math.Max(1, filter.PageNumber);
        var pageSize = Math.Clamp(filter.PageSize, 1, 50);

        var query = context.Portfolios
            .Include(p => p.Stocks)
            .AsNoTracking()
            .AsQueryable();

        query = query.ApplySearch(filter.Search, filter.SearchBy);
        
        var totalRecords = await query.CountAsync(cancellationToken);
        
        query = query.ApplySort(
            string.IsNullOrWhiteSpace(filter.SortBy) ? "Name" : filter.SortBy);
        
        var portfolios = await query
            .ApplyPagination(pageNumber, pageSize)
            .Select(p => new PortfolioStockDto
            {
                PortfolioId = p.Id,
                UserId = p.UserId,
                PortfolioName = p.Name ?? "",
                TickerSymbols = p.Stocks.Select(s => s.TickerSymbol).ToList()
                
            })
            .ToListAsync(cancellationToken);
        
        return new PagedResponse<PortfolioStockDto>
        {
            Data = portfolios,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalRecords,
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize)
        };
    }
    
    public async Task<IReadOnlyList<Portfolio>> GetAllPortfoliosAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        return await context.Portfolios
            .Include(p => p.Stocks)
            .ToListAsync();
    }

    public async Task<bool> PortfolioNameExistsAsync(string userId, string portfolioName, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        
        return await context.Portfolios
            .AnyAsync(p => p.Name == portfolioName && p.UserId == userId, cancellationToken);
    }

    public async Task<IList<string>> GetUninitializedStocks(IList<string> tickerSymbols, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        
        return await context.Stocks
            .Where(s => tickerSymbols.Contains(s.TickerSymbol))
            .Where(s => s.IsInitialized == false)
            .Select(s => s.TickerSymbol)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CreatePortfolioAsync(Portfolio portfolio, IList<string> tickerSymbols, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var stocks =  await context.Stocks
            .Where(s => tickerSymbols.Contains(s.TickerSymbol))
            .ToListAsync(cancellationToken);

        portfolio.AssignStocks(stocks);

        context.Portfolios.Add(portfolio);
        return await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PortfolioStockDto>?> GetPortfoliosByUserIdAsync(string userId, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        
        var userSetting = await context.UserSettings
            .FirstOrDefaultAsync(us => us.UserId == userId, cancellationToken);
        
        var portfolios = await context.Portfolios
            .Where(p => p.UserId == userId)
            .Include(p => p.Stocks.Where(s => s.IsInitialized))
            .ToListAsync(cancellationToken);

        var noPortfolios = portfolios.Count == 0;
        var noUserSettings = userSetting == null;
        
        var defaultPortfolioId = userSetting?.DefaultPortfolioId;
        
        if (noPortfolios)
        {
            var defaultPortfolio = new Portfolio
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = "Default Portfolio"
            };
            
            defaultPortfolioId = defaultPortfolio.Id;

            var createPortfolioResult = await CreatePortfolioAsync(defaultPortfolio, [], cancellationToken);
            if (createPortfolioResult <= 0)
            {
                throw new InvalidOperationException("Failed to create default portfolio.");
            }
        }

        if (noUserSettings)
        {
            userSetting = new UserSetting
            {
                UserId = userId,
                DefaultPortfolioId = defaultPortfolioId
            };
            
            context.UserSettings.Add(userSetting);
            var userSettingsInitResult = await context.SaveChangesAsync(cancellationToken);
            if (userSettingsInitResult <= 0)
            {
                throw new InvalidOperationException("Failed to initialize user settings.");
            }
        }
        
        return portfolios
            .Select(p => new PortfolioStockDto
            {
                PortfolioId = p.Id,
                UserId = p.UserId,
                PortfolioName = p.Name ?? "",
                IsDefault = p.Id == defaultPortfolioId,
                TickerSymbols = p.Stocks.Select(s => s.TickerSymbol).ToList()
            }).ToList();
    }
    
    public async Task<bool> PortfolioNameExistsAsync(string userId, Guid portfolioId, string portfolioName, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        
        return await context.Portfolios
            .AnyAsync(p => p.UserId == userId &&  p.Id != portfolioId && p.Name == portfolioName, cancellationToken);
    }

    public async Task<Portfolio> EditPortfolioAsync(Guid portfolioId, string portfolioName, IList<string> tickerSymbols, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        
        var portfolioToEdit = await context.Portfolios
            .Include(p => p.Stocks)
            .FirstOrDefaultAsync(p => p.Id == portfolioId, cancellationToken);

        if (portfolioToEdit == null)
        {
            throw new InvalidOperationException($"Portfolio with id: {portfolioId} was not found");
        }
        
        var tickers = tickerSymbols
            .Where(ts => !string.IsNullOrWhiteSpace(ts))
            .Select(ts => ts.Trim())
            .ToList();
        
        var uninitializedStocksInRequest = await context.Stocks
            .Where(s => tickers.Contains(s.TickerSymbol))
            .Where(s => s.IsInitialized == false)
            .Select(s => s.TickerSymbol)
            .ToListAsync(cancellationToken);

        if (uninitializedStocksInRequest.Count != 0)
        {
            throw new StocksNotInitializedException(uninitializedStocksInRequest);
        }
        
        var stocks = await context.Stocks
            .Where(s => tickers.Contains(s.TickerSymbol))
            .ToListAsync(cancellationToken);
        
        portfolioToEdit.Stocks.Clear();
        foreach (var s in stocks)
        {
            portfolioToEdit.Stocks.Add(s);
        }
        
        portfolioToEdit.Name = portfolioName;
        
        context.Portfolios.Update(portfolioToEdit);
        var result = await context.SaveChangesAsync(cancellationToken);
        
        return result <= 0 ? throw new Exception("Failed to update portfolio.") : portfolioToEdit;
    }
    
    public async Task<int> DeletePortfolioAsync(Guid portfolioId, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        
        var portfolio = await context.Portfolios.FirstOrDefaultAsync(p => p.Id == portfolioId, cancellationToken);
        if (portfolio == null)
        {
            throw new InvalidOperationException($"Portfolio with id: {portfolioId} not found.");
        }
        
        context.Portfolios.Remove(portfolio);
        return await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Portfolio?> GetPortfolioByIdAsync(Guid portfolioId, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        
        return await context.Portfolios
            .FirstOrDefaultAsync(p => p.Id == portfolioId, cancellationToken);
    }
    
    public async Task<PortfolioStockDto?> GetPortfolioByIdForUserAsync(string userId, Guid portfolioId, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var userSetting = await context.UserSettings
            .FirstOrDefaultAsync(us => us.UserId == userId, cancellationToken);

        var defaultPortfolioId = userSetting?.DefaultPortfolioId;

        var portfolio = await context.Portfolios
            .Where(p => p.Id == portfolioId && p.UserId == userId)
            .Include(p => p.Stocks.Where(s => s.IsInitialized))
            .Select(p => new PortfolioStockDto
            {
                PortfolioId = p.Id,
                UserId = p.UserId,
                PortfolioName = p.Name ?? "",
                IsDefault = p.Id == defaultPortfolioId,
                TickerSymbols = p.Stocks.Select(s => s.TickerSymbol).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        return portfolio;
    }

    public async Task<int> GetPortfolioCountByUserIdAsync(string userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        return await context.Portfolios
            .CountAsync(p => p.UserId == userId);
    }
}