using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.DTOs.Common;
using StockMapSvelte.Domain.Entities;

namespace StockMapSvelte.Application.Abstractions.Repositories;

public interface IPortfolioRepository : IRepository<Portfolio>
{
    Task<int> CreatePortfolioAsync(Portfolio portfolio, IList<string> tickerSymbols, CancellationToken cancellationToken);
    Task<IReadOnlyList<Portfolio>?> GetPortfoliosByUserIdAsync(string userId, CancellationToken cancellationToken);
    Task<bool> PortfolioNameExistsAsync(string userId, Guid portfolioId, string portfolioName, CancellationToken cancellationToken);
    Task<int> EditPortfolioAsync(Guid portfolioId, string portfolioName, IList<string> tickerSymbols, CancellationToken cancellationToken);
    Task<int> DeletePortfolioAsync(Guid portfolioId, CancellationToken cancellationToken);
    Task<Portfolio?> GetPortfolioByIdAsync(Guid portfolioId, CancellationToken cancellationToken);
    Task<PortfolioDto?> GetPortfolioByIdForUserAsync(string userId, Guid portfolioId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Portfolio>> GetAllPortfoliosAsync();
    Task<bool> PortfolioNameExistsAsync(string userId, string portfolioName, CancellationToken cancellationToken);
    Task<int> GetPortfolioCountByUserIdAsync(string userId, CancellationToken cancellationToken);

    // REFACTORED
    public Task<Portfolio?> GetByIdAsync(string userId, Guid portfolioId, CancellationToken cancellationToken);
    Task<PagedResponse<PortfolioDto>> GetAllAsync(QueryFilter filter, CancellationToken cancellationToken);
    public Task<int> GetCountByUserIdAsync(string userId, CancellationToken cancellationToken);
    public Task<bool> NameExistsAsync(string userId, string portfolioName, CancellationToken cancellationToken);
    public Task<bool> NameExistsAsync(string userId, Guid portfolioId, string portfolioName, CancellationToken cancellationToken);
    public new Task<Portfolio?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    public Task<IReadOnlyList<Portfolio>> GetForUserAsync(string userId, CancellationToken cancellationToken);
}