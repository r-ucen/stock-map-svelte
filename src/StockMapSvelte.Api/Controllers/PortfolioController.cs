using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using StockMapSvelte.Api.Requests;
using StockMapSvelte.Application.Abstractions.Facades;
using StockMapSvelte.Application.UseCases.PortfolioUseCases.Commands;
using StockMapSvelte.Application.UseCases.PortfolioUseCases.Queries;

namespace StockMapSvelte.Api.Controllers;

[EnableRateLimiting("DataPolicy")]
[ApiController]
[Authorize] 
[Route("portfolios")]
public class PortfolioController : Controller
{
    private readonly IPortfolioFacade _portfolioFacade;

    public PortfolioController(IPortfolioFacade portfolioFacade)
    {
        _portfolioFacade = portfolioFacade;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetAllPortfolios()
    {
        var portfolios = await _portfolioFacade.GetAllPortfolioStockViewModelsAsync();
        return Ok(portfolios);
    }

    [HttpGet]
    [Route("me")]
    public async Task<IActionResult> GetCurrentUserPortfolios()
    {
        var portfolios = await _portfolioFacade.GetPortfoliosByUserIdAsync();
        return Ok(portfolios);
    }

    [HttpDelete]
    [Route("{portfolioId:guid}")]
    public async Task<IActionResult> DeletePortfolio(Guid portfolioId)
    {
        await _portfolioFacade.DeletePortfolioAsync(portfolioId);
        return NoContent();
    }

    [HttpPut]
    [Route("{portfolioId:guid}")]
    public async Task<IActionResult> EditPortfolio(Guid portfolioId, EditPortfolioRequest request)
    {
        var cmd = new EditPortfolioCommand
        {
            PortfolioId = portfolioId,
            PortfolioName = request.PortfolioName,
            TickerSymbols = request.TickerSymbols
        };
        
        var editedPortfolio = await _portfolioFacade.EditPortfolioAsync(cmd);
        return Ok(editedPortfolio);
    }
    
    [HttpGet]
    [Route("{portfolioId:guid}")]
    public async Task<IActionResult> GetPortfolioById(Guid portfolioId)
    {
        var query = new GetPortfolioByIdQuery
        {
            PortfolioId = portfolioId
        };
        
        var portfolio = await _portfolioFacade.GetPortfolioStockByIdAsync(query);
        
        return Ok(portfolio);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePortfolio(CreatePortfolioRequest request)
    {
        var cmd = new CreatePortfolioCommand
        {
            PortfolioName = request.PortfolioName,
            TickerSymbols = request.TickerSymbols
        };

        var portfolio = await _portfolioFacade.CreatePortfolioAsync(cmd);
        
        return CreatedAtAction(
            nameof(GetPortfolioById),
            new { portfolioId = portfolio.PortfolioId },
            portfolio
        );
    }
}