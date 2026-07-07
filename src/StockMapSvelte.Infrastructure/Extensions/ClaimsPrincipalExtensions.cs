using System.Security.Claims;

namespace StockMapSvelte.Infrastructure.Extensions;

public static class ClaimsPrincipalExtensions
{
    extension (ClaimsPrincipal principal)
    {
        public string? UserId => principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}