using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.DTOs.Common;
using StockMapSvelte.Application.UseCases.PortfolioUseCases.Commands;
using StockMapSvelte.Application.UseCases.PortfolioUseCases.Queries;

namespace StockMapSvelte.Application.Abstractions.Facades;

public interface IPortfolioFacade
{
    Task<PagedResponse<PortfolioStockDto>> GetAllPortfolioStockViewModelsQueriedAsync(QueryFilter filter, CancellationToken cancellationToken);
    Task<PortfolioStockDto> CreatePortfolioAsync(CreatePortfolioCommand cmd, CancellationToken cancellationToken);
    Task<IReadOnlyList<PortfolioStockDto>> GetPortfoliosByUserIdAsync(CancellationToken cancellationToken);
    Task<PortfolioStockDto> EditPortfolioAsync(EditPortfolioCommand cmd, CancellationToken cancellationToken);
    Task DeletePortfolioAsync(Guid portfolioId, CancellationToken cancellationToken);
    Task<PortfolioStockDto> GetPortfolioStockByIdAsync(GetPortfolioByIdQuery query, CancellationToken cancellationToken);
}