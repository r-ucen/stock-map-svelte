using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using StockMapSvelte.Api.Requests;
using StockMapSvelte.Api.Requests.Portfolio;
using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Facades;
using StockMapSvelte.Application.DTOs.Common;
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
    private readonly IUserContext _userContext;

    public PortfolioController(IPortfolioFacade portfolioFacade, IUserContext userContext)
    {
        _portfolioFacade = portfolioFacade;
        _userContext = userContext;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetAllPortfoliosQueried([FromQuery] QueryFilter filter, CancellationToken cancellationToken = default)
    {
        var portfolios = await _portfolioFacade.GetAllPortfolioStockViewModelsQueriedAsync(filter, cancellationToken);
        return Ok(portfolios);
    }

    [HttpGet]
    [Route("me")]
    public async Task<IActionResult> GetCurrentUserPortfolios(CancellationToken cancellationToken = default)
    {
        var currentUserId = await _userContext.GetCurrentUserIdAsync();
        var query = new GetPortfoliosByUserIdQuery(currentUserId);
        
        var portfolios = await _portfolioFacade.GetPortfoliosByUserIdAsync(query, cancellationToken);
        return Ok(portfolios);
    }

    [HttpDelete]
    [Route("{portfolioId:guid}")]
    public async Task<IActionResult> DeletePortfolio(Guid portfolioId, CancellationToken cancellationToken = default)
    {
        var cmd = new DeletePortfolioCommand(portfolioId);
        
        await _portfolioFacade.DeletePortfolioAsync(cmd, cancellationToken);
        return NoContent();
    }

    [HttpPut]
    [Route("{portfolioId:guid}")]
    public async Task<IActionResult> EditPortfolio(Guid portfolioId, EditPortfolioRequest request, CancellationToken cancellationToken = default)
    {
        var cmd = new EditPortfolioCommand
        {
            PortfolioId = portfolioId,
            PortfolioName = request.PortfolioName,
            TickerSymbols = request.TickerSymbols
        };
        
        var editedPortfolio = await _portfolioFacade.EditPortfolioAsync(cmd, cancellationToken);
        return Ok(editedPortfolio);
    }
    
    [HttpGet]
    [Route("{portfolioId:guid}")]
    public async Task<IActionResult> GetPortfolioById(Guid portfolioId, CancellationToken cancellationToken = default)
    {
        var query = new GetPortfolioByIdQuery(portfolioId);
        
        var portfolio = await _portfolioFacade.GetPortfolioStockByIdAsync(query, cancellationToken);
        
        return Ok(portfolio);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePortfolio(CreatePortfolioRequest request, CancellationToken cancellationToken = default)
    {
        var cmd = new CreatePortfolioCommand
        (
            request.PortfolioName,
            request.TickerSymbols
        );

        var portfolio = await _portfolioFacade.CreatePortfolioAsync(cmd, cancellationToken);
        
        return CreatedAtAction(
            nameof(GetPortfolioById),
            new { portfolioId = portfolio.PortfolioId },
            portfolio
        );
    }
}