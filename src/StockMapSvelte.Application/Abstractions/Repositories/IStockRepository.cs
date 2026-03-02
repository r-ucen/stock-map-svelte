using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Domain.Entities;

namespace StockMapSvelte.Application.Abstractions.Repositories;

public interface IStockRepository
{
    Task<IReadOnlyList<StockDto>?> GetAllStocksAsync();
    Task<bool> StockExistsAsync(string ticker);
    Task<bool> StockExistsAsync(Guid stockId);
    Task<int> CreateStockAsync(Stock stock);
    Task<int> DeleteStockAsync(Guid stockId);
    Task<StockDto?> GetStockViewModelByIdAsync(Guid stockId);
    Task<int> EditStockAsync(Guid stockId, string ticker);
    Task<IReadOnlyList<StockDto>> GetPossibleToAddStocksAsync(string filter, IList<string> stocksInPortfolio, CancellationToken cancellationToken);
}