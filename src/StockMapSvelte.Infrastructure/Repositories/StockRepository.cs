using Microsoft.EntityFrameworkCore;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.DTOs.Common;
using StockMapSvelte.Application.Exceptions.Stock;
using StockMapSvelte.Domain.Entities;
using StockMapSvelte.Infrastructure.Database;
using StockMapSvelte.Infrastructure.Extensions;
using StockMapSvelte.Infrastructure.Extensions.Stock;

namespace StockMapSvelte.Infrastructure.Repositories;

public class StockRepository : IStockRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public StockRepository(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }
    
    public async Task<bool> StockExistsAsync(string ticker, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        return await context.Stocks
            .AsNoTracking()
            .AnyAsync(s => s.TickerSymbol == ticker, cancellationToken);
    }
    
    public async Task<bool> StockExistsAsync(Guid stockId, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        return await context.Stocks
            .AsNoTracking()
            .AnyAsync(s => s.Id == stockId, cancellationToken);
    }

    public async Task<int> CreateStockAsync(Stock stock)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        context.Stocks.Add(stock);
        return await context.SaveChangesAsync();
    }

    public async Task<int> DeleteStockAsync(Guid stockId, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        if (stockId == Guid.Empty)
        {
            throw new InvalidStockIdException("Invalid stock id.", stockId);
        }

        var stock = await context.Stocks.FindAsync([stockId], cancellationToken);
        if (stock is null)
        {
            throw new InvalidOperationException($"Stock with id {stockId} not found");
        }

        context.Stocks.Remove(stock);
        return await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> EditStockAsync(Guid stockId, string ticker, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        
        var existingStock = await context.Stocks.FindAsync([stockId], cancellationToken);
        if (existingStock == null)
        {
            throw new InvalidOperationException($"Stock with id: '{stockId}' was not found.");
        }
        
        existingStock.TickerSymbol = ticker;

        return await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<StockDto>?> GetAllStocksAsync(CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        
        var stocks = await context.Stocks.AsNoTracking().OrderBy(s => s.TickerSymbol).ToListAsync(cancellationToken);
        
        return stocks.Select(s => new StockDto
        (
            s.Id,
            s.TickerSymbol
        )).ToList();
    }
    
    public async Task<PagedResponse<StockDto>> GetAllStocksAsyncQueried(QueryFilter filter, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        
        var pageNumber = Math.Max(1, filter.PageNumber);
        var pageSize = Math.Clamp(filter.PageSize, 1, 50);

        var query = context.Stocks.AsNoTracking().AsQueryable();
        
        // apply search filter
        query = query.ApplySearch(filter.Search);

        // count total records AFTER filtering, BEFORE pagination
        var totalRecords = await query.CountAsync(cancellationToken);

        // apply sorting (default to TickerSymbol if not specified)
        query = query.ApplySort(
            string.IsNullOrWhiteSpace(filter.SortBy) ? "TickerSymbol" : filter.SortBy);

        // apply pagination and project to DTOs
        var stocks = await query
            .ApplyPagination(pageNumber, pageSize)
            .Select(m => new StockDto(m.Id, m.TickerSymbol))
            .ToListAsync(cancellationToken);

        return new PagedResponse<StockDto>
        {
            Data = stocks,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalRecords,
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize)
        };
    }
    
    public async Task<IReadOnlyList<StockDto>> GetPossibleToAddStocksAsync(string filter, IList<string> stocksInPortfolio, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        
        var upperTickersInPortfolio = stocksInPortfolio
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => t.Trim().ToUpperInvariant())
            .ToList();

        return await context.Stocks
            .AsNoTracking()
            .Where(s => s.IsInitialized == true)
            .Where(s => !upperTickersInPortfolio.Contains(s.TickerSymbol))
            .Where(s => s.TickerSymbol.Contains(filter))
            .OrderBy(s => s.TickerSymbol)
            .Take(20)
            .Select(s => new StockDto
            (
                s.Id,
                s.TickerSymbol
            ))
            .ToListAsync(cancellationToken);
    }

    public async Task<StockDto?> GetStockViewModelByIdAsync(Guid stockId, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        
        var stock = await context.Stocks.AsNoTracking().FirstOrDefaultAsync(s => s.Id == stockId, cancellationToken);
        return stock == null ? null : new StockDto(stockId, stock.TickerSymbol);
    }

    public async Task<IReadOnlyList<Stock>> GetUninitializedStocksAsync(CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        
        return await context.Stocks
            .AsNoTracking()
            .Where(s => s.IsInitialized == false)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> MarkStocksAsInitializedAsync(IEnumerable<Guid> stockIds, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var uninitializedStocks =  context.Stocks
            .Where(s => stockIds.Contains(s.Id));

        return await uninitializedStocks
            .ExecuteUpdateAsync(x => x.SetProperty(
                s => s.IsInitialized, true
                ), cancellationToken);
    }
}