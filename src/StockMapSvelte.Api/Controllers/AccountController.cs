using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using StockMapSvelte.Api.Requests;
using StockMapSvelte.Application.Abstractions.Facades;

namespace StockMapSvelte.Api.Controllers;

[EnableRateLimiting("DataPolicy")]
[ApiController]
[Authorize]
[Route("account")]
public class AccountController : Controller
{
    private readonly IAccountActionFacade _accountActionFacade;
    
    public AccountController(IAccountActionFacade accountActionFacade)
    {
        _accountActionFacade = accountActionFacade;
    }
    
    [HttpGet("info")]
    public async Task<IActionResult> GetAccountInfo()
    {
        var accountInfo = await _accountActionFacade.GetAccountInfoAsync();
        return Ok(accountInfo);
    }
    
    [HttpDelete("me")]
    public async Task<IActionResult> DeleteMyAccount()
    {
        await _accountActionFacade.DeleteMyAccountAsync();
        return NoContent();
    }

    [HttpPost("set-password")]
    public async Task<IActionResult> SetPassword([FromBody] SetPasswordRequest request)
    {
        await _accountActionFacade.SetPasswordAsync(request.NewPassword);
        return Ok();
    }
    
    [HttpDelete("remove-google-external-login")]
    public async Task<IActionResult> RemoveGoogleExternalLogin()
    {
        await _accountActionFacade.RemoveGoogleExternalLoginAsync();
        return NoContent();
    }
}