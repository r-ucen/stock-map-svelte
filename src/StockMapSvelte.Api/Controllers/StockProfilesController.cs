using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockMapSvelte.Application.Abstractions.Facades;

namespace StockMapSvelte.Api.Controllers;

[ApiController]
[Authorize]
[Route("stock-profile")]
public class StockProfilesController : Controller
{
    private readonly IStockProfileFacade _stockProfileFacade;
    
    public StockProfilesController(IStockProfileFacade stockProfileFacade)
    {
        _stockProfileFacade = stockProfileFacade;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    [Route("all")]
    public async Task<IActionResult> GetAllStockProfiles()
    {
        var stockProfiles = await _stockProfileFacade.GetAllStockProfilesAsync();
        return Ok(stockProfiles);
    }
}