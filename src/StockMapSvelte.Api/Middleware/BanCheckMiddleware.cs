using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Hybrid;
using StockMapSvelte.Infrastructure.Identity;
using StockMapSvelte.Infrastructure.Repositories.Cached.CacheManagement;

namespace StockMapSvelte.Api.Middleware;

public class BanCheckMiddleware
{
    private readonly RequestDelegate _next;
    private readonly HybridCache _cache;

    public BanCheckMiddleware(RequestDelegate next, HybridCache cache)
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
                var options = new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromMinutes(15),
                    LocalCacheExpiration = TimeSpan.FromMinutes(15)
                };
                
                var isBanned = await _cache.GetOrCreateAsync(
                    CacheKeys.User.Banned(userId),
                    async cancel =>
                    {
                        var user = await userManager.FindByIdAsync(userId);
                        return user is not null && await userManager.IsLockedOutAsync(user);
                    },
                    options: options
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