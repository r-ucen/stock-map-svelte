using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using StockMapSvelte.Application.Abstractions.Facades;
using StockMapSvelte.Application.DTOs.Common;
using StockMapSvelte.Application.UseCases.StockUseCases.Commands;
using StockMapSvelte.Application.UseCases.StockUseCases.Queries;

namespace StockMapSvelte.Api.Controllers;

[EnableRateLimiting("DataPolicy")]
[ApiController]
[Authorize] 
[Route("stocks")]
public class StockController : Controller
{
    private readonly IStockFacade _stockFacade;

    public StockController(IStockFacade stockFacade)
    {
        _stockFacade = stockFacade;
    }

    [HttpGet]
    [Authorize(Roles = "Admin, Manager")]
    [Route("{stockId:guid}")]
    public async Task<IActionResult> GetStock(Guid stockId, CancellationToken cancellationToken = default)
    {
        var stock = await _stockFacade.GetStockViewModelByIdAsync(stockId, cancellationToken);
        return Ok(stock);
    }
    
    [HttpGet]
    [Route("possible-to-add")]
    public async Task<IActionResult> GetPossibleToAddStocks([FromQuery] string filter, [FromQuery] IList<string> stocksInPortfolio, CancellationToken cancellationToken = default)
    {
        var query = new GetPossibleToAddStocksQuery(filter, stocksInPortfolio);
        
        var stocks = await _stockFacade.GetPossibleToAddStocksAsync(query, cancellationToken);
        return Ok(stocks);
    }
    
    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetAllStocksQueried([FromQuery] QueryFilter filter, CancellationToken cancellationToken = default)
    {
        var stocks = await _stockFacade.GetAllStocksQueriedAsync(filter, cancellationToken);
        return Ok(stocks);
    }

    [HttpPut]
    [Authorize(Roles = "Admin,Manager")]
    [Route("{stockId:guid}")]
    public async Task<IActionResult> EditStock(Guid stockId, string ticker, CancellationToken cancellationToken = default)
    {
        var cmd = new EditStockCommand(stockId, ticker);
        
        await _stockFacade.EditStockAsync(cmd, cancellationToken);
        return NoContent();
    }

    [HttpDelete]
    [Authorize(Roles = "Admin,Manager")]
    [Route("{stockId:guid}")]
    public async Task<IActionResult> DeleteStock(Guid stockId, CancellationToken cancellationToken = default)
    {
        var cmd = new DeleteStockCommand(stockId);
        
        await _stockFacade.DeleteStockAsync(cmd, cancellationToken);
        return NoContent();
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> CreateStock([FromBody] CreateStockCommand cmd, CancellationToken cancellationToken = default)
    {
        var createdStock = await _stockFacade.CreateStockAsync(cmd, cancellationToken);
        
        return CreatedAtAction(
            nameof(GetStock),
            new { stockId = createdStock.Id },
            createdStock);
    }
    
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    [Route("batch")]
    public async Task<IActionResult> CreateStocksBatch([FromBody] CreateStocksCommand cmd, CancellationToken cancellationToken = default)
    {
        var createdStocks = await _stockFacade.CreateStocksAsync(cmd, cancellationToken);
        return Ok(createdStocks);
    }
}