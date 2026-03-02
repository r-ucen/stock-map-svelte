using Microsoft.EntityFrameworkCore;
using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.Exceptions.Stock;
using StockMapSvelte.Domain.Entities;
using StockMapSvelte.Infrastructure.Database;

namespace StockMapSvelte.Infrastructure.Repositories;

public class StockRepository : IStockRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly IStockClient _stockClient;

    public StockRepository(IDbContextFactory<ApplicationDbContext> contextFactory, IStockClient stockClient)
    {
        _contextFactory = contextFactory;
        _stockClient = stockClient;
    }
    
    public async Task<bool> StockExistsAsync(string ticker)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        return await context.Stocks
            .AsNoTracking()
            .AnyAsync(s => s.TickerSymbol == ticker);
    }
    
    public async Task<bool> StockExistsAsync(Guid stockId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        return await context.Stocks
            .AsNoTracking()
            .AnyAsync(s => s.Id == stockId);
    }

    public async Task<int> CreateStockAsync(Stock stock)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        context.Stocks.Add(stock);
        return await context.SaveChangesAsync();
    }

    public async Task<int> DeleteStockAsync(Guid stockId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        if (stockId == Guid.Empty)
        {
            throw new InvalidStockIdException("Invalid stock id.", stockId);
        }

        var stock = await context.Stocks.FindAsync(stockId);
        if (stock is null)
        {
            throw new InvalidOperationException($"Stock with id {stockId} not found");
        }

        context.Stocks.Remove(stock);
        return await context.SaveChangesAsync();
    }

    public async Task<int> EditStockAsync(Guid stockId, string ticker)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var existingStock = await context.Stocks.FindAsync(stockId);
        if (existingStock == null)
        {
            throw new InvalidOperationException($"Stock with id: '{stockId}' was not found.");
        }
        
        existingStock.TickerSymbol = ticker;

        return await context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<StockDto>?> GetAllStocksAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var stocks = await context.Stocks.AsNoTracking().OrderBy(s => s.TickerSymbol).ToListAsync();
        
        return stocks.Select(s => new StockDto
        {
            Id = s.Id,
            TickerSymbol = s.TickerSymbol
        }).ToList();
    }
    
    public async Task<IReadOnlyList<StockDto>> GetPossibleToAddStocksAsync(string filter, IList<string> stocksInPortfolio, CancellationToken cancellationToken)
    {
        await using var _dbContext = await _contextFactory.CreateDbContextAsync();
        
        var upperTickersInPortfolio = stocksInPortfolio
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => t.Trim().ToUpperInvariant())
            .ToList();

        return await _dbContext.Stocks
            .AsNoTracking()
            .Where(s => !upperTickersInPortfolio.Contains(s.TickerSymbol!))
            .Where(s => s.TickerSymbol!.Contains(filter))
            .OrderBy(s => s.TickerSymbol)
            .Take(20)
            .Select(s => new StockDto
            {
                Id = s.Id,
                TickerSymbol = s.TickerSymbol ?? string.Empty
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<StockDto?> GetStockViewModelByIdAsync(Guid stockId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var stock = await context.Stocks.AsNoTracking().FirstOrDefaultAsync(s => s.Id == stockId);
        if (stock == null)
        {
            return null;
        }
        return new StockDto
        {
            Id = stockId,
            TickerSymbol = stock.TickerSymbol
        };
    }
}