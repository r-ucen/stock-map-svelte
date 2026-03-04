using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Domain.Entities;
using StockMapSvelte.Infrastructure.Database;
using StockMapSvelte.Infrastructure.Identity;

namespace StockMapSvelte.Infrastructure.Repositories;

public class PortfolioRepository : IPortfolioRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public PortfolioRepository(IDbContextFactory<ApplicationDbContext> contextFactory, UserManager<ApplicationUser> userManager)
    {
        _contextFactory = contextFactory;
    }

    public async Task<IReadOnlyList<PortfolioStockDto>?> GetAllPortfolioStockViewModelsAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var portfolios = await context.Portfolios
            .Include(p => p.Stocks)
            .ToListAsync();

        return portfolios
            .Select(p => new PortfolioStockDto
            {
                PortfolioId = p.Id,
                UserId = p.UserId,
                PortfolioName = p.Name ?? "",
                TickerSymbols = p.Stocks.Select(s => s.TickerSymbol).ToList() ?? []
            }).ToList();
    }
    
    public async Task<IReadOnlyList<Portfolio>> GetAllPortfoliosAsync()
    {
        await using var _dbContext = await _contextFactory.CreateDbContextAsync();
        
        return await _dbContext.Portfolios
            .Include(p => p.Stocks)
            .ToListAsync();
    }

    public async Task<bool> PortfolioNameExistsAsync(string userId, string portfolioName)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        return await context.Portfolios
            .AnyAsync(p => p.Name == portfolioName && p.UserId == userId);
    }

    public async Task<int> CreatePortfolioAsync(Portfolio portfolio, IList<string> tickerSymbols)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == portfolio.UserId);
        
        if (user == null)
        {
            throw new InvalidOperationException("Authenticated user not found.");
        }
        
        var upperTickerSymbols = tickerSymbols
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => t.Trim().ToUpperInvariant())
            .ToList();

        var stocks =  await context.Stocks
            .Where(s => upperTickerSymbols.Contains(s.TickerSymbol!))
            .ToListAsync();

        portfolio.Stocks = stocks;

        context.Portfolios.Add(portfolio);
        return await context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<PortfolioStockDto>?> GetPortfoliosByUserIdAsync(string userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var userSetting = await context.UserSettings
            .FirstOrDefaultAsync(us => us.UserId == userId);

        if (userSetting == null)
        {
            userSetting = new UserSetting
            {
                UserId = userId,
                DefaultPortfolioId = null
            };
            
            context.UserSettings.Add(userSetting);
            var userSettingsInitResult = await context.SaveChangesAsync();
            if (userSettingsInitResult <= 0)
            {
                throw new InvalidOperationException("Failed to initialize user settings.");
            }
        }
        
        var defaultPortfolioId = userSetting.DefaultPortfolioId;
        
        var portfolios = await context.Portfolios
            .Where(p => p.UserId == userId)
            .Include(p => p.Stocks)
            .ToListAsync();
        
        return portfolios
            .Select(p => new PortfolioStockDto
            {
                PortfolioId = p.Id,
                UserId = p.UserId,
                PortfolioName = p.Name ?? "",
                IsDefault = p.Id == defaultPortfolioId,
                TickerSymbols = p.Stocks.Select(s => s.TickerSymbol).ToList() ?? []
            }).ToList();
    }
    
    public async Task<bool> PortfolioNameExistsAsync(string userId, Guid portfolioId, string portfolioName)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        return await context.Portfolios
            .AnyAsync(p => p.UserId == userId &&  p.Id != portfolioId && p.Name == portfolioName);
    }

    public async Task<int> EditPortfolioAsync(Guid portfolioId, string portfolioName, IList<string> tickerSymbols)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var portfolioToEdit = await context.Portfolios
            .Include(p => p.Stocks)
            .FirstOrDefaultAsync(p => p.Id == portfolioId);

        if (portfolioToEdit == null)
        {
            throw new InvalidOperationException($"Portfolio with id: {portfolioId} was not found");
        }
        
        var tickers = tickerSymbols?
            .Where(ts => !string.IsNullOrWhiteSpace(ts))
            .Select(ts => ts.Trim())
            .ToList() ?? [];
        
        var stocks = await context.Stocks
            .Where(s => tickers.Contains(s.TickerSymbol!))
            .ToListAsync();
        
        portfolioToEdit.Stocks.Clear();
        foreach (var s in stocks)
        {
            portfolioToEdit.Stocks.Add(s);
        }
        
        portfolioToEdit.Name = portfolioName;
        
        context.Portfolios.Update(portfolioToEdit);
        return await context.SaveChangesAsync();
    }
    
    public async Task<int> DeletePortfolioAsync(Guid portfolioId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var portfolio = await context.Portfolios.FirstOrDefaultAsync(p => p.Id == portfolioId);
        if (portfolio == null)
        {
            throw new InvalidOperationException($"Portfolio with id: {portfolioId} not found.");
        }
        
        context.Portfolios.Remove(portfolio);
        return await context.SaveChangesAsync();
    }

    public async Task<Portfolio?> GetPortfolioByIdAsync(Guid portfolioId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        return await context.Portfolios
            .FirstOrDefaultAsync(p => p.Id == portfolioId);
    }
    
    public async Task<PortfolioStockDto?> GetPortfolioByIdForUserAsync(string userId, Guid portfolioId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var userSetting = await context.UserSettings
            .FirstOrDefaultAsync(us => us.UserId == userId);

        var defaultPortfolioId = userSetting?.DefaultPortfolioId;

        var portfolio = await context.Portfolios
            .Where(p => p.Id == portfolioId && p.UserId == userId)
            .Include(p => p.Stocks)
            .Select(p => new PortfolioStockDto
            {
                PortfolioId = p.Id,
                UserId = p.UserId,
                PortfolioName = p.Name ?? "",
                IsDefault = p.Id == defaultPortfolioId,
                TickerSymbols = p.Stocks.Select(s => s.TickerSymbol).ToList()
            })
            .FirstOrDefaultAsync();

        return portfolio;
    }

    public async Task<int> GetPortfolioCountByUserIdAsync(string userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        return await context.Portfolios
            .CountAsync(p => p.UserId == userId);
    }
}