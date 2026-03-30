using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using StockMapSvelte.Application.Abstractions.Facades;
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
    public async Task<IActionResult> GetStock(Guid stockId)
    {
        var stock = await _stockFacade.GetStockViewModelByIdAsync(stockId);
        return Ok(stock);
    }
    
    [HttpGet]
    [Route("possible-to-add")]
    public async Task<IActionResult> GetPossibleToAddStocks([FromQuery] string filter, [FromQuery] IList<string> stocksInPortfolio, CancellationToken cancellationToken)
    {
        var query = new GetPossibleToAddStocksQuery
        {
            Filter = filter,
            StocksInPortfolio = stocksInPortfolio,
            CancellationToken = cancellationToken
        };
        
        var stocks = await _stockFacade.GetPossibleToAddStocksAsync(query);
        return Ok(stocks);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetAllStocks()
    {
        var stocks = await _stockFacade.GetAllStocksAsync();
        return Ok(stocks);
    }

    [HttpPut]
    [Authorize(Roles = "Admin,Manager")]
    [Route("{stockId:guid}")]
    public async Task<IActionResult> EditStock(Guid stockId, string ticker)
    {
        var cmd = new EditStockCommand()
        {
            Id = stockId,
            TickerSymbol = ticker
        };
        
        await _stockFacade.EditStockAsync(cmd);
        return NoContent();
    }

    [HttpDelete]
    [Authorize(Roles = "Admin,Manager")]
    [Route("{stockId:guid}")]
    public async Task<IActionResult> DeleteStock(Guid stockId)
    {
        var cmd = new DeleteStockCommand
        {
            StockId = stockId
        };
        
        await _stockFacade.DeleteStockAsync(cmd);
        return NoContent();
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> CreateStock([FromBody] CreateStockCommand cmd)
    {
        var createdStock = await _stockFacade.CreateStockAsync(cmd);
        
        return CreatedAtAction(
            nameof(GetStock),
            new { stockId = createdStock.Id },
            createdStock);
    }
}