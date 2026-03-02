using StockMapSvelte.Application.DTOs;

namespace StockMapSvelte.Application.Abstractions.Facades;

public interface IPortfolioFacade
{
    Task<IReadOnlyList<PortfolioStockDto>> GetAllPortfolioStockViewModelsAsync();
    Task CreatePortfolioAsync(string portfolioName, IList<string> tickers);
    Task<IReadOnlyList<PortfolioStockDto>> GetPortfoliosByUserIdAsync();
    Task EditPortfolioAsync(PortfolioStockDto portfolio);
    Task DeletePortfolioAsync(Guid portfolioId);
}