using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Domain.Entities;

namespace StockMapSvelte.Application.Abstractions.Repositories;

public interface IPortfolioRepository
{
    Task<IReadOnlyList<PortfolioStockDto>?> GetAllPortfolioStockViewModelsAsync();
    Task<int> CreatePortfolioAsync(Portfolio portfolio, IList<string> tickerSymbols);
    Task<IReadOnlyList<PortfolioStockDto>?> GetPortfoliosByUserIdAsync(string userId);
    Task<bool> PortfolioNameExistsAsync(string userId, Guid portfolioId, string portfolioName);
    Task<Portfolio> EditPortfolioAsync(Guid portfolioId, string portfolioName, IList<string> tickerSymbols);
    Task<int> DeletePortfolioAsync(Guid portfolioId);
    Task<Portfolio?> GetPortfolioByIdAsync(Guid portfolioId);
    Task<PortfolioStockDto?> GetPortfolioByIdForUserAsync(string userId, Guid portfolioId);
    Task<IReadOnlyList<Portfolio>> GetAllPortfoliosAsync();
    Task<bool> PortfolioNameExistsAsync(string userId, string portfolioName);
    Task<int> GetPortfolioCountByUserIdAsync(string userId);
}