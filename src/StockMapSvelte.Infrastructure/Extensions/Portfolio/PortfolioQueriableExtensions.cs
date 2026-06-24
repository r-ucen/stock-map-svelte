using Microsoft.EntityFrameworkCore;

namespace StockMapSvelte.Infrastructure.Extensions.Portfolio;

public static class PortfolioQueriableExtensions
{
    public static IQueryable<Domain.Entities.Portfolio> ApplySearch(this IQueryable<Domain.Entities.Portfolio> query,
        string? search, string? searchBy)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return query;
        }
        
        var targetColumn = searchBy?.Trim().ToLower();

        return targetColumn switch
        {
            "userid" => query.Where(p =>
                EF.Functions.ILike(p.UserId ?? "", $"%{search}%")),
            "name" => query.Where(p =>
                EF.Functions.ILike(p.Name ?? "", $"%{search}%")),
            "stocks" => query.Where(p =>
                p.Stocks.Any(s =>
                    EF.Functions.ILike(s.TickerSymbol, $"%{search}%"))),

            _ => query.Where(p =>
                EF.Functions.ILike(p.UserId, $"%{search}%"))
        };
    }
}