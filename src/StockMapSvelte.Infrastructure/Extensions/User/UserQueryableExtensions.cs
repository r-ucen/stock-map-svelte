using Microsoft.EntityFrameworkCore;
using StockMapSvelte.Infrastructure.Identity;

namespace StockMapSvelte.Infrastructure.Extensions.User;

public static class UserQueryableExtensions
{
    public static IQueryable<ApplicationUser> ApplySearch(this IQueryable<ApplicationUser> query, string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return query;
        }

        return query.Where(u =>
            EF.Functions.ILike(u.UserName ?? "", $"%{search}%") || 
            EF.Functions.ILike(u.Email ?? "", $"%{search}%") || 
            EF.Functions.ILike(u.PhoneNumber ?? "", $"%{search}%") ||
            EF.Functions.ILike(u.LockoutEnabled.ToString(), $"%{search}%") ||
            EF.Functions.ILike(u.PhoneNumberConfirmed.ToString(), $"%{search}%") ||
            EF.Functions.ILike(u.EmailConfirmed.ToString(), $"%{search}%") ||
            EF.Functions.ILike(u.TwoFactorEnabled.ToString(), $"%{search}%") ||
            EF.Functions.ILike(u.AccessFailedCount.ToString(), $"%{search}%"));
    }
}