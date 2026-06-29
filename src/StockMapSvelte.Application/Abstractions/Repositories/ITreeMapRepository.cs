using StockMapSvelte.Application.DTOs;

namespace StockMapSvelte.Application.Abstractions.Repositories;

public interface ITreeMapRepository
{
    Task<TreemapDataDto> GetTreemapDataViewModelByIdAsync(Guid portfolioId,  CancellationToken cancellationToken);
}