using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockMapSvelte.Application.Abstractions.Facades;

namespace StockMapSvelte.Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class TreemapDataController : Controller
{
    private readonly ITreeMapFacade _treeMapFacade;
    
    public TreemapDataController(ITreeMapFacade treeMapFacade)
    {
        this._treeMapFacade = treeMapFacade;
    }

    [HttpGet("{portfolioId:guid}")]
    public async Task<IActionResult> GetTreemapDataByPortfolioId(Guid portfolioId)
    {
        var treemapData = await _treeMapFacade.GetTreemapDataViewModelByIdAsync(portfolioId);
        return  Ok(treemapData);
    }
}