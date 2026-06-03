using StockMapSvelte.Application.UseCases.TreeMapUseCases.Handlers;
using StockMapSvelte.Application.Abstractions.Facades;
using StockMapSvelte.Application.DTOs;

namespace StockMapSvelte.Application.Facade;

public class TreeMapFacade : ITreeMapFacade
{
    private readonly GetTreemapDataHandler _getTreemapDataHandler;
    
    public TreeMapFacade(GetTreemapDataHandler getTreemapDataHandler)
    {
        _getTreemapDataHandler = getTreemapDataHandler;
    }
    
    public async Task<TreemapDataDto> GetTreemapDataViewModelByIdAsync(Guid portfolioId, CancellationToken cancellationToken)
    {
        return await _getTreemapDataHandler.Handle(portfolioId,  cancellationToken);
    }
}