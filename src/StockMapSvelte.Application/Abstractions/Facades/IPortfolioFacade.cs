using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.DTOs.Common;
using StockMapSvelte.Application.UseCases.PortfolioUseCases.Commands;
using StockMapSvelte.Application.UseCases.PortfolioUseCases.Queries;

namespace StockMapSvelte.Application.Abstractions.Facades;

public interface IPortfolioFacade
{
    public Task<PortfolioDto> ImportPortfolioFromTrading212(ImportPortfolioFromTrading212Command cmd, CancellationToken cancellationToken);
    Task<PagedResponse<PortfolioDto>> GetAllPortfolioStockViewModelsQueriedAsync(QueryFilter filter, CancellationToken cancellationToken);
    Task<PortfolioDto> CreatePortfolioAsync(CreatePortfolioCommand cmd, CancellationToken cancellationToken);
    Task<IReadOnlyList<PortfolioDto>> GetPortfoliosByUserIdAsync(GetPortfoliosByUserIdQuery query, CancellationToken cancellationToken);
    Task<PortfolioDto> EditPortfolioAsync(EditPortfolioCommand cmd, CancellationToken cancellationToken);
    Task DeletePortfolioAsync(DeletePortfolioCommand cmd, CancellationToken cancellationToken);
    Task<PortfolioDto> GetPortfolioStockByIdAsync(GetPortfolioByIdQuery query, CancellationToken cancellationToken);
}