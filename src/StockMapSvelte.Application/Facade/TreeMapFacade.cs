using StockMapSvelte.Application.UseCases.TreeMapUseCases.Handlers;
using StockMapSvelte.Application.Abstractions.Facades;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.UseCases.TreeMapUseCases.Queries;

namespace StockMapSvelte.Application.Facade;

public class TreeMapFacade : ITreeMapFacade
{
    private readonly GetTreemapDataHandler _getTreemapDataHandler;
    
    public TreeMapFacade(GetTreemapDataHandler getTreemapDataHandler)
    {
        _getTreemapDataHandler = getTreemapDataHandler;
    }
    
    public async Task<TreemapDataDto> GetTreemapDataViewModelByIdAsync(GetTreemapDataQuery query, CancellationToken cancellationToken)
    {
        return await _getTreemapDataHandler.Handle(query, cancellationToken);
    }
}