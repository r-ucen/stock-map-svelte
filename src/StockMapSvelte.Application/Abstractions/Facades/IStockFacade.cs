using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.DTOs.Common;
using StockMapSvelte.Application.UseCases.StockUseCases.Commands;
using StockMapSvelte.Application.UseCases.StockUseCases.Queries;

namespace StockMapSvelte.Application.Abstractions.Facades;

public interface IStockFacade
{
    Task<PagedResponse<StockDto>> GetAllStocksQueriedAsync(QueryFilter filter, CancellationToken cancellationToken);
    Task<StockDto> GetStockViewModelByIdAsync(Guid stockId, CancellationToken cancellationToken);
    Task<StockDto> CreateStockAsync(CreateStockCommand cmd, CancellationToken cancellationToken);
    Task<CreateStocksResponse> CreateStocksAsync(CreateStocksCommand cmd, CancellationToken cancellationToken);
    Task DeleteStockAsync(DeleteStockCommand cmd, CancellationToken cancellationToken);
    Task EditStockAsync(EditStockCommand cmd, CancellationToken cancellationToken);
    Task<IReadOnlyList<StockDto>> GetPossibleToAddStocksAsync(GetPossibleToAddStocksQuery query, CancellationToken cancellationToken);
}