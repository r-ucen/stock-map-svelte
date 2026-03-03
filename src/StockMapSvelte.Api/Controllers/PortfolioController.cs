using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockMapSvelte.Application.Abstractions.Facades;

namespace StockMapSvelte.Api.Controllers;

[ApiController]
[Authorize]
[Route("portfolio")]
public class PortfolioController : Controller
{
    private readonly IPortfolioFacade _portfolioFacade;
    
    public PortfolioController(IPortfolioFacade portfolioFacade)
    {
        _portfolioFacade = portfolioFacade;
    }
    
    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    [Route("all")]
    public async Task<IActionResult> GetAllPortfolios()
    {
        var portfolios = await _portfolioFacade.GetAllPortfolioStockViewModelsAsync();
        return Ok(portfolios);
    }
}