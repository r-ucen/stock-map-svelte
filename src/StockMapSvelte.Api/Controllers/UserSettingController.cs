using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockMapSvelte.Application.Abstractions.Facades;

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
    
}