using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockMapSvelte.Application.Abstractions.Facades;

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
    
    


}