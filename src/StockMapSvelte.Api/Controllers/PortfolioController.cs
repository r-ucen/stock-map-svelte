using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockMapSvelte.Application.Abstractions.Facades;
using StockMapSvelte.Application.UseCases.PortfolioUseCases.Commands;

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

    [HttpGet]
    [Route("currentUser")]
    public async Task<IActionResult> GetCurrentUserPortfolios()
    {
        var portfolios = await _portfolioFacade.GetPortfoliosByUserIdAsync();
        return Ok(portfolios);
    }

    [HttpDelete]
    [Route("{portfolioId}")]
    public async Task<IActionResult> DeletePortfolio(Guid portfolioId)
    {
        await _portfolioFacade.DeletePortfolioAsync(portfolioId);
        return NoContent();
    }
}