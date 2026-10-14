using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using StockMapSvelte.Api.Requests.Account;
using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Facades;
using StockMapSvelte.Application.UseCases.AccountActionUseCases.Commands;
using StockMapSvelte.Application.UseCases.AccountActionUseCases.Queries;

namespace StockMapSvelte.Api.Controllers;

[EnableRateLimiting("DataPolicy")]
[ApiController]
[Authorize]
[Route("account")]
public class AccountController : Controller
{
    private readonly IAccountActionFacade _accountActionFacade;
    private readonly IUserContext _userContext;
    
    public AccountController(IAccountActionFacade accountActionFacade, IUserContext userContext)
    {
        _accountActionFacade = accountActionFacade;
        _userContext = userContext;
    }
    
    /// <description>
    /// Returns info about the current authenticated user's account - HasPasswordConfigured, HasExternalLoginConfigured, ExternalLogins, IsAdmin, IsManager, IsCustomer, Email, IsEmailConfirmed.
    /// </description>
    /// <returns></returns>
    [HttpGet("info")]
    public async Task<IActionResult> GetAccountInfo()
    {
        var currentUserId = await _userContext.GetCurrentUserIdAsync();
        var query = new GetAccountInfoQuery(currentUserId);
        
        var accountInfo = await _accountActionFacade.GetAccountInfoAsync(query);
        return Ok(accountInfo);
    }
    
    /// <description>
    /// Deletes the whole account for the authenticated user.
    /// </description>
    /// <returns></returns>
    [HttpDelete("me")]
    public async Task<IActionResult> DeleteMyAccount()
    {
        var currentUserId = await _userContext.GetCurrentUserIdAsync();
        var command = new DeleteAccountCommand(currentUserId);
        
        await _accountActionFacade.DeleteMyAccountAsync(command);
        return NoContent();
    }

    /// <description>
    /// Used for adding a password to passwordless account.
    /// </description>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("set-password")]
    public async Task<IActionResult> SetPassword([FromBody] SetPasswordRequest request)
    {
        var currentUserId = await _userContext.GetCurrentUserIdAsync();
        var command = new SetPasswordCommand(currentUserId, request.NewPassword);
        
        await _accountActionFacade.SetPasswordAsync(command);
        return Ok();
    }
    
    /// <description>
    /// Removes google external login if the account has password set.
    /// </description>
    /// <returns></returns>
    [HttpDelete("remove-google-external-login")]
    public async Task<IActionResult> RemoveGoogleExternalLogin()
    {
        var currentUserId = await _userContext.GetCurrentUserIdAsync();
        var command = new RemoveGoogleExternalLoginCommand(currentUserId);
        
        await _accountActionFacade.RemoveGoogleExternalLoginAsync(command);
        return NoContent();
    }
}