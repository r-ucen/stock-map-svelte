using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.DTOs.Common;
using StockMapSvelte.Domain.Entities;

namespace StockMapSvelte.Application.Abstractions.Repositories;

public interface IStockProfileRepository : IRepository<StockProfile>
{
    Task<int> SaveAsync(IEnumerable<StockProfile> profiles, CancellationToken cancellationToken);
    Task<PagedResponse<StockStockProfileDto>> GetAllStockProfilesAsync(QueryFilter filter, CancellationToken cancellationToken);
}