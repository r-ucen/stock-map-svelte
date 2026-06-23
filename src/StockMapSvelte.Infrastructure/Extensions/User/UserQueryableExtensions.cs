using Microsoft.EntityFrameworkCore;
using StockMapSvelte.Infrastructure.Identity;

namespace StockMapSvelte.Infrastructure.Extensions.User;

public static class UserQueryableExtensions
{
    public static IQueryable<ApplicationUser> ApplySearch(this IQueryable<ApplicationUser> query, string? search, string? searchBy)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return query;
        }
        
        var targetColumn = searchBy?.Trim().ToLower();

        return targetColumn switch
        {
            "username" => query.Where(u =>
                EF.Functions.ILike(u.UserName ?? "", $"%{search}%")),
            "email" => query.Where(u =>
                EF.Functions.ILike(u.Email ?? "", $"%{search}%")),
            "phone" => query.Where(u =>
                EF.Functions.ILike(u.PhoneNumber ?? "", $"%{search}%")),
            "lockoutenabled" => query.Where(u =>
                EF.Functions.ILike(u.LockoutEnabled.ToString(), $"%{search}%")),
            "phonenumberconfirmed" => query.Where(u =>
                EF.Functions.ILike(u.PhoneNumberConfirmed.ToString(), $"%{search}%")),
            "emailconfirmed" => query.Where(u =>
                EF.Functions.ILike(u.EmailConfirmed.ToString(), $"%{search}%")),
            "twofactorenabled" => query.Where(u =>
                EF.Functions.ILike(u.TwoFactorEnabled.ToString(), $"%{search}%")),
            "accessfailedcount" => query.Where(u =>
                EF.Functions.ILike(u.AccessFailedCount.ToString(), $"%{search}%")),

            _ => query.Where(u =>
                EF.Functions.ILike(u.Email ?? "", $"%{search}%"))
        };
    }
}