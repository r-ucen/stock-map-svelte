using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.DTOs.Common;
using StockMapSvelte.Domain.Entities;
using StockMapSvelte.Infrastructure.Database;
using StockMapSvelte.Infrastructure.Extensions;
using StockMapSvelte.Infrastructure.Extensions.Stock;

namespace StockMapSvelte.Infrastructure.Repositories;

public class StockRepository : Repository<Stock>, IStockRepository
{
    public StockRepository(ApplicationDbContext dbContext) : base(dbContext) { }
    
    public async Task<IList<string>> GetMissingStocksAsync(IList<string> tickerSymbols, CancellationToken cancellationToken)
    {
        var existingSymbols = await DbSet
            .AsNoTracking()
            .Where(s => tickerSymbols.Contains(s.TickerSymbol))
            .Select(s => s.TickerSymbol)
            .ToListAsync(cancellationToken);

        return tickerSymbols
            .Except(existingSymbols, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
    
    public async Task<bool> StockExistsAsync(string ticker, CancellationToken cancellationToken)
    {
        return await DbSet
            .AsNoTracking()
            .AnyAsync(s => s.TickerSymbol == ticker, cancellationToken);
    }
    
    public async Task<bool> StockExistsAsync(Guid stockId, CancellationToken cancellationToken)
    {
        return await DbSet
            .AsNoTracking()
            .AnyAsync(s => s.Id == stockId, cancellationToken);
    }

    public async Task<int> CreateStockAsync(Stock stock)
    {
        DbSet.Add(stock);
        return await Context.SaveChangesAsync();
    }

    public async Task<int> DeleteStockAsync(Guid stockId, CancellationToken cancellationToken)
    {
        return await DbSet
            .Where(s => s.Id == stockId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<int> EditStockAsync(Guid stockId, string ticker, CancellationToken cancellationToken)
    {
        var existingStock = await DbSet.FindAsync([stockId], cancellationToken);
        if (existingStock == null)
        {
            throw new InvalidOperationException($"Stock with id: '{stockId}' was not found.");
        }
        
        existingStock.Update(ticker);

        return await Context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task<PagedResponse<StockDto>> GetAllStocksAsyncQueried(QueryFilter filter, CancellationToken cancellationToken)
    {
        var pageNumber = Math.Max(1, filter.PageNumber);
        var pageSize = Math.Clamp(filter.PageSize, 1, 50);

        var query = DbSet.AsNoTracking().AsQueryable();
        
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
        var upperTickersInPortfolio = stocksInPortfolio
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => t.Trim().ToUpperInvariant())
            .ToList();

        return await DbSet
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
        var stock = await DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == stockId, cancellationToken);
        return stock == null ? null : new StockDto(stockId, stock.TickerSymbol);
    }

    public async Task<IReadOnlyList<Stock>> GetUninitializedStocksAsync(CancellationToken cancellationToken)
    {
        return await DbSet
            .AsNoTracking()
            .Where(s => s.IsInitialized == false)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> MarkStocksAsInitializedAsync(IEnumerable<Guid> stockIds, CancellationToken cancellationToken)
    {
        var uninitializedStocks =  DbSet
            .Where(s => stockIds.Contains(s.Id));

        return await uninitializedStocks
            .ExecuteUpdateAsync(x => x.SetProperty(
                s => s.IsInitialized, true
                ), cancellationToken);
    }
    
    public async Task<IList<string>> GetUninitializedStocks(IList<string> tickerSymbols, CancellationToken cancellationToken)
    {
        return await DbSet
            .AsNoTracking()
            .Where(s => tickerSymbols.Contains(s.TickerSymbol))
            .Where(s => s.IsInitialized == false)
            .Select(s => s.TickerSymbol)
            .ToListAsync(cancellationToken);
    }
    
    // REFACTORED

    public async Task<IReadOnlyList<string>> GetMissingTickerSymbolsAsync(IList<string> tickerSymbols, CancellationToken cancellationToken)
    {
        var existingSymbols = await DbSet
            .AsNoTracking()
            .Where(s => tickerSymbols.Contains(s.TickerSymbol))
            .Select(s => s.TickerSymbol)
            .ToListAsync(cancellationToken);

        return tickerSymbols
            .Except(existingSymbols, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public async Task<IList<string>> GetUninitializedTickerSymbols(IList<string> tickerSymbols,
        CancellationToken cancellationToken)
    {
        return await DbSet
            .AsNoTracking()
            .Where(s => tickerSymbols.Contains(s.TickerSymbol) && s.IsInitialized == false)
            .Select(s => s.TickerSymbol)
            .ToListAsync(cancellationToken);
    }
}