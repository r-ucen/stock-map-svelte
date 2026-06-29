using Microsoft.AspNetCore.Identity;
using StockMapSvelte.Application.DTOs;

namespace StockMapSvelte.Application.Abstractions;

public interface IIdentityService
{
    Task LogOutAsync();
    Task<ExternalLoginResponse> HandleExternalLoginAsync();
    Task<bool> LinkAccountWithPasswordAsync(string email, string password);
    public Task<bool> HasPasswordConfiguredAsync(string userId);
    public Task<bool> SetAccountPasswordAsync(string userId, string password);
    public Task<IList<UserLoginInfo>> GetExternalLoginsAsync(string userId);
    public Task<UserLoginInfo?> GetGoogleLoginAsync(string userId);
    public Task<bool> RemoveGoogleExternalLoginAsync(string userId, UserLoginInfo googleLogin);
    public Task<bool> DeleteAccountAsync(string userId);
    public Task<bool> IsUserInRoleAsync(string userId, string role);
    public Task<bool> DoesUserComplyWithPolicyAsync(string userId, string policyName);
    public Task<string?> GetUserEmailAsync(string userId);
    public Task<bool> IsEmailConfirmedAsync(string userId);
    public Task<IList<string>> GetUserRolesAsync(string userId);
    public Task<bool> UpdateUserRoles(string userId, string[] roles);
    public Task<bool> IsUserTheLastAdminAsync(string userId);
    public Task<bool> UserExistsAsync(string userId);
}