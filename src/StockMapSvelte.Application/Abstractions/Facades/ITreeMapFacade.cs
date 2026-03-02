using StockMapSvelte.Application.DTOs;

namespace StockMapSvelte.Application.Abstractions.Facades;

public interface ITreeMapFacade
{
    Task<TreemapDataDto> GetTreemapDataViewModelByIdAsync(Guid portfolioId);
}