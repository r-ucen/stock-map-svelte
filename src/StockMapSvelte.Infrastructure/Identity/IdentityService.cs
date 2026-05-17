using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.Enums;

namespace StockMapSvelte.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }
    
    public async Task LogOutAsync()
    { 
        await _signInManager.SignOutAsync();
    }

    public async Task<ExternalLoginResponse> HandleExternalLoginAsync()
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            return new ExternalLoginResponse()
            {
                Result = ExternalLoginResult.Failure
            };
        }
        
        var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
        // if the provider account exists and also exists in our database
        if (result.Succeeded || result.RequiresTwoFactor)
        {
            // do not prompt for 2fa code when using external provider
            if (result.RequiresTwoFactor)
            {
                var u = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                if (u != null)
                {
                    await _signInManager.SignInAsync(u, isPersistent: false);
                }
            }
            
            return new ExternalLoginResponse()
            {
                Result = ExternalLoginResult.Success
            };
        }
        
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(email))
        {
            return new ExternalLoginResponse()
            {
                Result = ExternalLoginResult.Failure
            };
        }
        
        var user = await _userManager.FindByEmailAsync(email);
        // if the user exists and is a local account, do not link automatically
        if (user != null && await _userManager.HasPasswordAsync(user))
        {
            return new ExternalLoginResponse()
            {
                Result = ExternalLoginResult.AccountExistsRequireLinking,
                Email = email
            };
        }
        
        // the user does not exist, in this case create and link the external provider
        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };
            await _userManager.CreateAsync(user);
        }
        
        await _userManager.AddLoginAsync(user, info);
        await _signInManager.SignInAsync(user, isPersistent: false);
        return new ExternalLoginResponse()
        {
            Result = ExternalLoginResult.Success
        };
    }

    public async Task<bool> LinkAccountWithPasswordAsync(string email, string password)
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null || info == null)
        {
            return false;
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
        if (!isPasswordValid)
        {
            return false;
        }
        
        var addResult = await _userManager.AddLoginAsync(user, info);

        if (!addResult.Succeeded)
        {
            return false;
        }
        await _signInManager.SignInAsync(user, isPersistent: false);
        return true;
    }
}