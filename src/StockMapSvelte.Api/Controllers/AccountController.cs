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
    
    [HttpGet("info")]
    public async Task<IActionResult> GetAccountInfo()
    {
        var currentUserId = await _userContext.GetCurrentUserIdAsync();
        var query = new GetAccountInfoQuery(currentUserId);
        
        var accountInfo = await _accountActionFacade.GetAccountInfoAsync(query);
        return Ok(accountInfo);
    }
    
    [HttpDelete("me")]
    public async Task<IActionResult> DeleteMyAccount()
    {
        var currentUserId = await _userContext.GetCurrentUserIdAsync();
        var command = new DeleteAccountCommand(currentUserId);
        
        await _accountActionFacade.DeleteMyAccountAsync(command);
        return NoContent();
    }

    [HttpPost("set-password")]
    public async Task<IActionResult> SetPassword([FromBody] SetPasswordRequest request)
    {
        var currentUserId = await _userContext.GetCurrentUserIdAsync();
        var command = new SetPasswordCommand(currentUserId, request.NewPassword);
        
        await _accountActionFacade.SetPasswordAsync(command);
        return Ok();
    }
    
    [HttpDelete("remove-google-external-login")]
    public async Task<IActionResult> RemoveGoogleExternalLogin()
    {
        var currentUserId = await _userContext.GetCurrentUserIdAsync();
        var command = new RemoveGoogleExternalLoginCommand(currentUserId);
        
        await _accountActionFacade.RemoveGoogleExternalLoginAsync(command);
        return NoContent();
    }
}