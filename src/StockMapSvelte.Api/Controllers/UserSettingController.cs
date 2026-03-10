using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockMapSvelte.Application.Abstractions.Facades;
using StockMapSvelte.Application.UseCases.UserSettingUseCases.Commands;

namespace StockMapSvelte.Api.Controllers;

[ApiController]
[Authorize]
[Route("user-settings")]
public class UserSettingController : Controller
{
    private readonly IUserSettingFacade _userSettingFacade;
    
    public UserSettingController(IUserSettingFacade userSettingFacade)
    {
        _userSettingFacade = userSettingFacade;
    }

    [HttpGet("default-portfolio")]
    public async Task<IActionResult> GetDefaultPortfolioId()
    {
        var defaultPortfolioId = await _userSettingFacade.GetDefaultPortfolioIdAsync();
        return Ok(defaultPortfolioId);
    }

    [HttpPut("default-portfolio")]
    public async Task<IActionResult> SetDefaultPortfolio([FromBody] SetPortfolioAsDefaultCommand command)
    {
        await _userSettingFacade.SetPortfolioAsDefaultAsync(command);
        return NoContent();
    }

    [HttpGet("toast-delay")]
    public async Task<IActionResult> GetToastAutoHideDelayMs()
    {
        var delayMs = await _userSettingFacade.GetToastAutoHideDelayMs();
        return Ok(delayMs);
    }

    [HttpPut("toast-delay")]
    public async Task<IActionResult> SetToastAutoHideDelayMs([FromBody] SetToastAutoHideDelayMsCommand command)
    {
        await _userSettingFacade.SetToastAutoHideDelayMsAsync(command);
        return NoContent();
    }

    [HttpGet("toast-auto-hide")]
    public async Task<IActionResult> GetToastAutoHideValue()
    {
        var value = await _userSettingFacade.GetToastAutoHideValueAsync();
        return Ok(value);
    }

    [HttpPut("toast-auto-hide")]
    public async Task<IActionResult> SetToastAutoHideValue([FromBody] SetToastAutoHideValueCommand command)
    {
        await _userSettingFacade.SetToastAutoHideValueAsync(command);
        return NoContent();
    }
}