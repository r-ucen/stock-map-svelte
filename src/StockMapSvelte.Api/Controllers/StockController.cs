using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockMapSvelte.Application.Abstractions.Facades;
using StockMapSvelte.Application.UseCases.StockUseCases.Queries;

namespace StockMapSvelte.Api.Controllers;

[ApiController]
[Authorize] 
[Route("stock")]
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
    [Route("all")]
    public async Task<IActionResult> GetAllStocks()
    {
        var stocks = await _stockFacade.GetAllStocksAsync();
        return Ok(stocks);
    }
}