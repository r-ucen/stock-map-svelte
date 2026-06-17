using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.Enums;

namespace StockMapSvelte.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _principalFactory;
    private readonly IAuthorizationService _authorizationService;

    public IdentityService(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        IUserClaimsPrincipalFactory<ApplicationUser> principalFactory,
        IAuthorizationService authorizationService)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _principalFactory = principalFactory;
        _authorizationService = authorizationService;
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
            return new ExternalLoginResponse
            (
                ExternalLoginResult.Failure,
                null
            );
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

            return new ExternalLoginResponse(
                ExternalLoginResult.Success,
                null
            );
        }
        
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(email))
        {
            return new ExternalLoginResponse
            (
                ExternalLoginResult.Failure,
                null
            );
        }
        
        var user = await _userManager.FindByEmailAsync(email);
        // if the user exists and is a local account, do not link automatically
        if (user != null && await _userManager.HasPasswordAsync(user))
        {
            return new ExternalLoginResponse
            (
                ExternalLoginResult.AccountExistsRequireLinking,
                email
            );
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
        return new ExternalLoginResponse
        (
            ExternalLoginResult.Success,
            null
        );
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
        
    public async Task<bool> HasPasswordConfiguredAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }
        return await _userManager.HasPasswordAsync(user);
    }

    public async Task<bool> SetAccountPasswordAsync(string userId, string password)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        if (await _userManager.HasPasswordAsync(user))
        {
            return false; 
        }

        var result = await _userManager.AddPasswordAsync(user, password);
        return result.Succeeded;
    }
    
    public async Task<IList<UserLoginInfo>> GetExternalLoginsAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new List<UserLoginInfo>();
        }

        return await _userManager.GetLoginsAsync(user);
    }

    public async Task<RemoveGoogleExternalLoginStatus> RemoveGoogleExternalLoginAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new RemoveGoogleExternalLoginStatus
            (
                false,
                "User not found."
            );
        }
        
        var hasPassword = await _userManager.HasPasswordAsync(user);
        if (!hasPassword)
        {
            return new RemoveGoogleExternalLoginStatus
            (
                false,
                "Cannot remove Google login because no password is set. Please set a password before removing the Google login."
            );
        }

        var logins = await _userManager.GetLoginsAsync(user);
        var googleLogin = logins.FirstOrDefault(l => l.LoginProvider == "GoogleOpenIdConnect");
        if (googleLogin == null)
        {
            return new RemoveGoogleExternalLoginStatus
            (
                false,
                "Google login not found for this user."
            );
        }

        var result = await _userManager.RemoveLoginAsync(user, googleLogin.LoginProvider, googleLogin.ProviderKey);
        if (!result.Succeeded)
        {
            return new RemoveGoogleExternalLoginStatus
            (
                false,
                "Failed to remove Google login."
            );
        }

        await _signInManager.SignOutAsync();
        return new RemoveGoogleExternalLoginStatus
        (
            true,
            "Google login removed successfully."
        );
    }
    
    public async Task<bool> DeleteAccountAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            return false;
        }
        await _signInManager.SignOutAsync();
        return true;

    }

    public async Task<bool> IsUserInRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }
        return await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> DoesUserComplyWithPolicyAsync(string userId, string policyName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }
        var principal = await _principalFactory.CreateAsync(user);
        var authorizationResult = await _authorizationService.AuthorizeAsync(principal, policyName);
        return authorizationResult.Succeeded;
    }

    public async Task<string?> GetUserEmailAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return null;
        }
        return await _userManager.GetEmailAsync(user);
    }

    public async Task<bool> IsEmailConfirmedAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }
        return await _userManager.IsEmailConfirmedAsync(user);
    }
}