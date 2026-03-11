using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockMapSvelte.Application.Abstractions.Facades;

namespace StockMapSvelte.Api.Controllers;

[ApiController]
[Authorize]
[Route("stock-profiles")]
public class StockProfilesController : Controller
{
    private readonly IStockProfileFacade _stockProfileFacade;
    
    public StockProfilesController(IStockProfileFacade stockProfileFacade)
    {
        _stockProfileFacade = stockProfileFacade;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetAllStockProfiles()
    {
        var stockProfiles = await _stockProfileFacade.GetAllStockProfilesAsync();
        return Ok(stockProfiles);
    }
}