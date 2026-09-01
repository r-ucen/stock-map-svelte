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

    public async Task<IReadOnlyList<Stock>> GetUninitializedAsync(CancellationToken cancellationToken)
    {
        return await DbSet
            .AsNoTracking()
            .Where(s => s.IsInitialized == false)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> MarkAsInitializedAsync(IEnumerable<Guid> stockIds, CancellationToken cancellationToken)
    {
        var uninitializedStocks =  DbSet
            .Where(s => stockIds.Contains(s.Id));

        return await uninitializedStocks
            .ExecuteUpdateAsync(x => x.SetProperty(
                s => s.IsInitialized, true
                ), cancellationToken);
    }
    
    public async Task<PagedResponse<StockDto>> GetAllAsync(QueryFilter filter, CancellationToken cancellationToken)
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

    public async Task<IList<string>> GetUninitializedTickerSymbolsAsync(IList<string> tickerSymbols,
        CancellationToken cancellationToken)
    {
        return await DbSet
            .AsNoTracking()
            .Where(s => tickerSymbols.Contains(s.TickerSymbol) && s.IsInitialized == false)
            .Select(s => s.TickerSymbol)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<bool> ExistsAsync(string ticker, CancellationToken cancellationToken)
    {
        return await DbSet
            .AnyAsync(s => s.TickerSymbol == ticker, cancellationToken);
    }
    
    public async Task<IReadOnlyList<StockDto>> GetPossibleToAddAsync(string filter, IList<string> stocksInPortfolio, CancellationToken cancellationToken)
    {
        return await DbSet
            .AsNoTracking()
            .Where(s => s.IsInitialized == true)
            .Where(s => !stocksInPortfolio.Contains(s.TickerSymbol))
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
}