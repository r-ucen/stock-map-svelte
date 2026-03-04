using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.UseCases.StockUseCases.Commands;
using StockMapSvelte.Application.UseCases.StockUseCases.Queries;

namespace StockMapSvelte.Application.Abstractions.Facades;

public interface IStockFacade
{
    Task<IReadOnlyList<StockDto>> GetAllStocksAsync();
    Task<StockDto> GetStockViewModelByIdAsync(Guid stockId);
    Task CreateStockAsync(string ticker);
    Task DeleteStockAsync(DeleteStockCommand cmd);
    Task EditStockAsync(EditStockCommand cmd);
    Task<IReadOnlyList<StockDto>> GetPossibleToAddStocksAsync(GetPossibleToAddStocksQuery query);
}