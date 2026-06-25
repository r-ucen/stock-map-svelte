using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.DTOs.Common;

namespace StockMapSvelte.Application.Abstractions.Facades;

public interface IStockProfileFacade
{
    Task<PagedResponse<StockStockProfileDto>> GetAllStockProfilesQueriedAsync(QueryFilter filter, CancellationToken cancellationToken);
}