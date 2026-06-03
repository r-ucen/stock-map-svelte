using StockMapSvelte.Application.DTOs;

namespace StockMapSvelte.Application.Abstractions.Facades;

public interface IStockProfileFacade
{
    Task<IReadOnlyList<StockStockProfileDto>> GetAllStockProfilesAsync(CancellationToken cancellationToken);
}