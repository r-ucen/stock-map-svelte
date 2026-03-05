using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockMapSvelte.Application.Abstractions.Facades;
using StockMapSvelte.Application.UseCases.UserSettingUseCases.Commands;

namespace StockMapSvelte.Api.Controllers;

[ApiController]
[Authorize]
[Route("user-setting")]
public class UserSettingController : Controller
{
    private readonly IUserSettingFacade _userSettingFacade;
    
    public UserSettingController(IUserSettingFacade userSettingFacade)
    {
        _userSettingFacade = userSettingFacade;
    }

    [HttpGet]
    [Route("default-portfolio-id")]
    public async Task<IActionResult> GetDefaultPortfolioId()
    {
        var defaultPortfolioId = await _userSettingFacade.GetDefaultPortfolioIdAsync();
        return Ok(defaultPortfolioId);
    }

    [HttpGet]
    [Route("toast-auto-hide-delay-ms")]
    public async Task<IActionResult> GetToastAutoHideDelayMs()
    {
        var delayMs = await _userSettingFacade.GetToastAutoHideDelayMs();
        return Ok(delayMs);
    }

    [HttpGet]
    [Route("toast-auto-hide-value")]
    public async Task<IActionResult> GetToastAutoHideValue()
    {
        var value = await _userSettingFacade.GetToastAutoHideValueAsync();
        return Ok(value);
    }

    [HttpPost]
    [Route("{portfolioId:guid}/set-default-portfolio")]
    public async Task<IActionResult> SetDefaultPortfolio(Guid portfolioId)
    {
        var cmd = new SetPortfolioAsDefaultCommand
        {
            PortfolioId = portfolioId
        };
        
        await _userSettingFacade.SetPortfolioAsDefaultAsync(cmd);
        return NoContent();
    }
}