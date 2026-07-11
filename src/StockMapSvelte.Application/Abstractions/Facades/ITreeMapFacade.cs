using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.UseCases.TreeMapUseCases.Queries;

namespace StockMapSvelte.Application.Abstractions.Facades;

public interface ITreeMapFacade
{
    Task<TreemapDataDto> GetTreemapDataViewModelByIdAsync(GetTreemapDataQuery query, CancellationToken cancellationToken);
}