using Microsoft.AspNetCore.Identity;
using StockMapSvelte.Application.Abstractions;

namespace StockMapSvelte.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly SignInManager<ApplicationUser> _signInManager;

    IdentityService(SignInManager<ApplicationUser> signInManager)
    {
        _signInManager = signInManager;
    }
    
    public Task LogOutAsync()
    {
        return _signInManager.SignOutAsync();
    }
}