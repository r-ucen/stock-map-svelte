using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using StockMapSvelte.Application.Abstractions.Facades;

namespace StockMapSvelte.Api.Controllers;

[ApiController]
[Authorize]
[Route("treemap-data")]
public class TreemapDataController : Controller
{
    private readonly ITreeMapFacade _treeMapFacade;
    
    public TreemapDataController(ITreeMapFacade treeMapFacade)
    {
        _treeMapFacade = treeMapFacade;
    }

    [EnableRateLimiting("TreeMapDataPolicy")]
    [HttpGet("{portfolioId:guid}")]
    public async Task<IActionResult> GetTreemapDataByPortfolioId(Guid portfolioId, CancellationToken cancellationToken = default)
    {
        var treemapData = await _treeMapFacade.GetTreemapDataViewModelByIdAsync(portfolioId, cancellationToken);
        return  Ok(treemapData);
    }
}