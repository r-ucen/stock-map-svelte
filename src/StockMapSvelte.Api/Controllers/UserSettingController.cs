using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using StockMapSvelte.Api.Requests.UserSetting;
using StockMapSvelte.Application.Abstractions.Facades;
using StockMapSvelte.Application.UseCases.UserSettingUseCases.Commands;

namespace StockMapSvelte.Api.Controllers;

[EnableRateLimiting("DataPolicy")]
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
    public async Task<IActionResult> GetDefaultPortfolioId(CancellationToken cancellationToken = default)
    {
        var defaultPortfolioId = await _userSettingFacade.GetDefaultPortfolioIdAsync(cancellationToken);
        return Ok(defaultPortfolioId);
    }

    [HttpPut("default-portfolio")]
    public async Task<IActionResult> SetDefaultPortfolio([FromBody] SetPortfolioAsDefaultRequest request, CancellationToken cancellationToken = default)
    {
        var cmd = new SetPortfolioAsDefaultCommand(request.PortfolioId);
        
        await _userSettingFacade.SetPortfolioAsDefaultAsync(cmd, cancellationToken);
        return NoContent();
    }
}