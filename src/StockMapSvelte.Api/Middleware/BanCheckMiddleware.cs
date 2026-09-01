using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Infrastructure.Cache;
using StockMapSvelte.Infrastructure.Identity;

namespace StockMapSvelte.Api.Middleware;

public class BanCheckMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ICacheService _cache;

    public BanCheckMiddleware(RequestDelegate next, ICacheService cache)
    {
        _next = next;
        _cache = cache;
    }

    // scoped userManager cannot be constructed once in the constructor, must be constructed each time InvokeAsync is called
    public async Task InvokeAsync(HttpContext context, UserManager<ApplicationUser> userManager)
    {
        
        if (context.Request.Path.Equals("/logout", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context); return;
        }
        
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrWhiteSpace(userId))
            {
                var isBanned = await _cache.GetOrCreateAsync(
                    CacheKeys.User.Banned(userId),
                    async _ =>
                    {
                        var user = await userManager.FindByIdAsync(userId);
                        return user is not null && await userManager.IsLockedOutAsync(user);
                    },
                    expiration: TimeSpan.FromMinutes(15),
                    localCacheExpiration: TimeSpan.FromMinutes(15)
                );

                if (isBanned)
                {
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                    await context.Response.WriteAsJsonAsync(
                        new
                        {
                            type = "https://tools.ietf.org/html/rfc9110#section-15.5.2",
                            title = "Unauthorized",
                            status = 401,
                            detail = "LockedOut"
                        }
                    );
                    return;
                }
            }
        }
        
        await _next(context);
    }
}