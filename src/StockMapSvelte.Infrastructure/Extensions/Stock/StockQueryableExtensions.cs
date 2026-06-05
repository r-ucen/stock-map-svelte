using Microsoft.EntityFrameworkCore;

namespace StockMapSvelte.Infrastructure.Extensions.Stock;

public static class StockQueryableExtensions
{
    public static IQueryable<Domain.Entities.Stock> ApplySearch(this IQueryable<Domain.Entities.Stock> query, string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return query;
        }

        return query.Where(m =>
            EF.Functions.ILike(m.TickerSymbol, $"%{search}%"));
    }
}