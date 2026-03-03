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
    private readonly ILogger<PortfolioController> _logger;
    
    public PortfolioController(IPortfolioFacade portfolioFacade, ILogger<PortfolioController> logger)
    {
        _portfolioFacade = portfolioFacade;
        _logger = logger;
    }
    
    [HttpGet]
    [Route("all")]
    public async Task<IActionResult> GetAllPortfolios()
    {
        try
        {
            var portfolios = await _portfolioFacade.GetAllPortfolioStockViewModelsAsync();
            return Ok(portfolios);
        }
        catch (UnauthorizedAccessException e)
        {
            _logger.LogWarning(e, "Unauthorized access attempt to get all portfolios.");
            return Forbid();
        }
    }
}