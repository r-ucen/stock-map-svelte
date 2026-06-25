using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.DTOs.Common;
using StockMapSvelte.Domain.Entities;

namespace StockMapSvelte.Application.Abstractions.Repositories;

public interface IStockProfileRepository
{
    Task<PagedResponse<StockStockProfileDto>> GetAllStockProfilesAsync(QueryFilter filter, CancellationToken cancellationToken);
    Task<int> SaveStockProfilesAsync(IEnumerable<StockProfile> profiles, CancellationToken cancellationToken);
}