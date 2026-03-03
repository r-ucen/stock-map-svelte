using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.UseCases.PortfolioUseCases.Commands;

namespace StockMapSvelte.Application.Abstractions.Facades;

public interface IPortfolioFacade
{
    Task<IReadOnlyList<PortfolioStockDto>> GetAllPortfolioStockViewModelsAsync();
    Task<PortfolioStockDto> CreatePortfolioAsync(CreatePortfolioCommand cmd);
    Task<IReadOnlyList<PortfolioStockDto>> GetPortfoliosByUserIdAsync();
    Task EditPortfolioAsync(EditPortfolioCommand cmd);
    Task DeletePortfolioAsync(Guid portfolioId);
}