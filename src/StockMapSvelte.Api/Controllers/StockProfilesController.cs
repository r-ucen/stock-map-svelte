using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using StockMapSvelte.Application.Abstractions.Facades;
using StockMapSvelte.Application.DTOs.Common;

namespace StockMapSvelte.Api.Controllers;

[EnableRateLimiting("DataPolicy")]
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
    public async Task<IActionResult> GetAllStockProfilesQueried([FromQuery] QueryFilter filter, CancellationToken cancellationToken = default)
    {
        var stockProfiles = await _stockProfileFacade.GetAllStockProfilesQueriedAsync(filter, cancellationToken);
        return Ok(stockProfiles);
    }
}