using Microsoft.AspNetCore.Http;
using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Infrastructure.Extensions;

namespace StockMapSvelte.Infrastructure.Identity;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public Task<string> GetCurrentUserIdAsync()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        var id = user?.UserId;

        return !string.IsNullOrEmpty(id) ? Task.FromResult(id) : throw new UnauthorizedAccessException("User is not authenticated.");
    }
}