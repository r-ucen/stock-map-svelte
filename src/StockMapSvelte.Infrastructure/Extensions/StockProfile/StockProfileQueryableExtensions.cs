using Microsoft.EntityFrameworkCore;

namespace StockMapSvelte.Infrastructure.Extensions.StockProfile;

public static class StockProfileQueryableExtensions
{
    public static IQueryable<Domain.Entities.Stock> ApplySearch(this IQueryable<Domain.Entities.Stock> query,
        string? search, string? searchBy)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return query;
        }
        
        var targetColumn = searchBy?.Trim().ToLower();

        return targetColumn switch
        {
            "fullname" => query.Where(s => EF.Functions.ILike(s.StockProfile.FullName ?? "", $"%{search}%")),
            "stockid"  => query.Where(s => EF.Functions.ILike(s.Id.ToString(), $"%{search}%")),
            "sector"   => query.Where(s => EF.Functions.ILike(s.StockProfile.Sector ?? "", $"%{search}%")),
            
            _ => query.Where(s => EF.Functions.ILike(s.StockProfile.FullName ?? "", $"%{search}%"))
        };
    }
}