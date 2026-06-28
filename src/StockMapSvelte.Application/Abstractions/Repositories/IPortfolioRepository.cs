using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.DTOs.Common;
using StockMapSvelte.Domain.Entities;

namespace StockMapSvelte.Application.Abstractions.Repositories;

public interface IPortfolioRepository
{
    public Task<IList<string>> GetUninitializedStocks(IList<string> tickerSymbols, CancellationToken cancellationToken);
    Task<PagedResponse<PortfolioStockDto>> GetAllPortfolioStockViewModelsAsync(QueryFilter filter, CancellationToken cancellationToken);
    Task<int> CreatePortfolioAsync(Portfolio portfolio, IList<string> tickerSymbols, CancellationToken cancellationToken);
    Task<IReadOnlyList<PortfolioStockDto>?> GetPortfoliosByUserIdAsync(string userId, CancellationToken cancellationToken);
    Task<bool> PortfolioNameExistsAsync(string userId, Guid portfolioId, string portfolioName, CancellationToken cancellationToken);
    Task<Portfolio> EditPortfolioAsync(Guid portfolioId, string portfolioName, IList<string> tickerSymbols, CancellationToken cancellationToken);
    Task<int> DeletePortfolioAsync(Guid portfolioId, CancellationToken cancellationToken);
    Task<Portfolio?> GetPortfolioByIdAsync(Guid portfolioId, CancellationToken cancellationToken);
    Task<PortfolioStockDto?> GetPortfolioByIdForUserAsync(string userId, Guid portfolioId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Portfolio>> GetAllPortfoliosAsync();
    Task<bool> PortfolioNameExistsAsync(string userId, string portfolioName, CancellationToken cancellationToken);
    Task<int> GetPortfolioCountByUserIdAsync(string userId);
}