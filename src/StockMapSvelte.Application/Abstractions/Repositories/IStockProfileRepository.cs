using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Domain.Entities;

namespace StockMapSvelte.Application.Abstractions.Repositories;

public interface IStockProfileRepository
{
    Task<IReadOnlyList<StockStockProfileDto>> GetAllStockProfilesAsync();
    Task<int> SaveStockProfilesAsync(IEnumerable<StockProfile> profiles);
}