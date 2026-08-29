using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.DTOs.Common;
using StockMapSvelte.Application.Enums;
using StockMapSvelte.Infrastructure.Extensions;
using StockMapSvelte.Infrastructure.Extensions.User;
using StockMapSvelte.Infrastructure.Repositories.Cached.CacheManagement;

namespace StockMapSvelte.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _principalFactory;
    private readonly IAuthorizationService _authorizationService;
    private readonly HybridCache _cache;

    public IdentityService(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        IUserClaimsPrincipalFactory<ApplicationUser> principalFactory,
        IAuthorizationService authorizationService,
        HybridCache cache)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _principalFactory = principalFactory;
        _authorizationService = authorizationService;
        _cache = cache;
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
                    await _signInManager.SignInAsync(u, isPersistent: true);
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
        await _signInManager.SignInAsync(user, isPersistent: true);
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
        await _signInManager.SignInAsync(user, isPersistent: true);
        return true;
    }
        
    public async Task<bool> HasPasswordConfiguredAsync(string userId)
    {
        var user = await GetUserById(userId);
        if (user == null) { return false; }
        
        return await _userManager.HasPasswordAsync(user);
    }

    public async Task<bool> SetAccountPasswordAsync(string userId, string password)
    {
        var user = await GetUserById(userId);
        if (user == null) { return false; }

        if (await _userManager.HasPasswordAsync(user))
        {
            return false; 
        }

        var result = await _userManager.AddPasswordAsync(user, password);
        return result.Succeeded;
    }
    
    public async Task<IList<UserLoginInfo>> GetExternalLoginsAsync(string userId)
    {
        var user = await GetUserById(userId);
        if (user == null) { return new List<UserLoginInfo>(); }

        return await _userManager.GetLoginsAsync(user);
    }

    public async Task<UserLoginInfo?> GetGoogleLoginAsync(string userId)
    {
        var user = await GetUserById(userId);
        if (user == null) { return null; }
        
        var logins = await _userManager.GetLoginsAsync(user);
        return logins.FirstOrDefault(l => l.LoginProvider == "GoogleOpenIdConnect");
    }

    public async Task<bool> RemoveGoogleExternalLoginAsync(string userId, UserLoginInfo googleLogin)
    {
        var user = await GetUserById(userId);
        if (user == null) { return false; }

        var result = await _userManager.RemoveLoginAsync(user, googleLogin.LoginProvider, googleLogin.ProviderKey);
        if (!result.Succeeded) { return false; }
        
        await _signInManager.SignOutAsync();
        return true;
    }
    
    public async Task<bool> DeleteAccountAsync(string userId)
    {
        var user = await GetUserById(userId);
        if (user == null) { return false; }

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded) { return false; }
        await _signInManager.SignOutAsync();
        return true;
    }

    public async Task<bool> IsUserInRoleAsync(string userId, string role)
    {
        var user = await GetUserById(userId);
        if (user == null) { return false; }
        
        return await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> DoesUserComplyWithPolicyAsync(string userId, string policyName)
    {
        var user = await GetUserById(userId);
        if (user == null) { return false; }
        
        var principal = await _principalFactory.CreateAsync(user);
        var authorizationResult = await _authorizationService.AuthorizeAsync(principal, policyName);
        return authorizationResult.Succeeded;
    }

    public async Task<string?> GetUserEmailAsync(string userId)
    {
        var user = await GetUserById(userId);
        if (user == null) { return null; }
        
        return await _userManager.GetEmailAsync(user);
    }

    public async Task<bool> IsEmailConfirmedAsync(string userId)
    {
        var user = await GetUserById(userId);
        if (user == null) { return false; }
        
        return await _userManager.IsEmailConfirmedAsync(user);
    }
    
    public async Task<IList<string>> GetUserRolesAsync(string userId)
    {
        var user = await GetUserById(userId);
        if (user == null) return new List<string>();
        return await _userManager.GetRolesAsync(user);
    }

    public async Task<bool> UpdateUserRoles(string userId, string[] roles)
    {
        var user = await GetUserById(userId);
        if (user == null) { return false; }

        var existingUserRoles = await _userManager.GetRolesAsync(user);
        var existingUserRolesNormalized = existingUserRoles.Where(r => r != "Customer");
        
        var rolesToRemove = existingUserRolesNormalized
            .Where(r => r is "Admin" or "Manager")
            .Except(roles)
            .ToArray();
        
        var rolesToAdd = roles
            .Except(existingUserRolesNormalized)
            .Where(r => r is "Admin" or "Manager")
            .ToArray();
        
        if (rolesToRemove.Length != 0)
        {
            var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            if (!removeResult.Succeeded) { return false; }
        }

        if (rolesToAdd.Length != 0)
        {
            var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
            if (!addResult.Succeeded)
            {
                return false;
            }
        }
        
        return true;
    }

    private async Task<int> GetUserCountInRoleAsync(string roleName)
    {
        var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);
        return usersInRole.Count;
    }

    public async Task<bool> IsUserTheLastAdminAsync(string userId)
    {
        var user = await GetUserById(userId);
        if (user == null) { return false; }
        
        var isUserAdmin = await _userManager.IsInRoleAsync(user, "Admin");
        var adminsCount = await GetUserCountInRoleAsync("Admin");

        return isUserAdmin && (adminsCount <= 1);
    }
    
    private async Task<ApplicationUser?> GetUserById(string userId)
    {
        return await _userManager.FindByIdAsync(userId);
    }

    public async Task<bool> UserExistsAsync(string userId)
    {
        return await _userManager.FindByIdAsync(userId) != null;
    }

    public async Task<bool> LockOutAsync(string userId)
    {
        var user = await GetUserById(userId);
        if (user == null) { return false; }

        var result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
        if (!result.Succeeded) { return false; }
        
        await _userManager.UpdateSecurityStampAsync(user);
        await _cache.SetAsync(CacheKeys.User.Banned(userId), true);
        return true;
    }
    
    public async Task<bool> UnlockAsync(string userId)
    {
        var user = await GetUserById(userId);
        if (user == null) { return false; }

        var result = await _userManager.SetLockoutEndDateAsync(user, null);
        if (!result.Succeeded) { return false; }
        
        await _cache.SetAsync(CacheKeys.User.Banned(userId), false);
        return true;
    }
    
    public async Task<bool> IsUserLockedOutAsync(string userId)
    {
        var user = await GetUserById(userId);
        if (user == null) { return false; }

        return await _userManager.IsLockedOutAsync(user);
    }

    public async Task<bool> DeleteUserAsync(string userId, CancellationToken cancellationToken)
    {
        if (userId == null) { throw new ArgumentNullException(userId); }
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user == null) { throw new InvalidOperationException($"The user with id: {userId} was not found"); }

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded;
    }

    public async Task<UserDto?> GetUserByIdAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user == null) { return null; }

        return new UserDto
        (
            user.Id,
            user.UserName ?? string.Empty,
            user.Email ?? string.Empty,
            await _userManager.GetRolesAsync(user),
            await _userManager.IsLockedOutAsync(user)
        );
    }

    public async Task<PagedResponse<UserDto>> GetAllUsersAsync(QueryFilter filter, CancellationToken cancellationToken)
    {
        var pageNumber = Math.Max(1, filter.PageNumber);
        var pageSize = Math.Clamp(filter.PageSize, 1, 50);
        
        var query = _userManager.Users.AsNoTracking().AsQueryable();
        
        query = query.ApplySearch(filter.Search, filter.SearchBy);
        
        var totalRecords = await query.CountAsync(cancellationToken);
        
        query = query.ApplySort(
            string.IsNullOrWhiteSpace(filter.SortBy) ? "Email" : filter.SortBy);
        
        var usersList = await query
            .ApplyPagination(pageNumber, pageSize)
            .ToListAsync(cancellationToken);
        
        var users = new List<UserDto>();
        
        foreach (var u in usersList)
        {
            var roles = await _userManager.GetRolesAsync(u);
    
            users.Add(new UserDto(
                u.Id, 
                u.Email ?? string.Empty,  
                u.UserName ?? string.Empty, 
                roles,
                await _userManager.IsLockedOutAsync(u)
            ));
        }
        
        return new PagedResponse<UserDto>
        {
            Data = users,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalRecords,
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize)
        };
    }
}