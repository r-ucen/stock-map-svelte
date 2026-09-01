using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.DTOs.Common;
using StockMapSvelte.Domain.Entities;

namespace StockMapSvelte.Application.Abstractions.Repositories;

public interface IPortfolioRepository : IRepository<Portfolio>
{
    public Task<Portfolio?> GetByIdAsync(string userId, Guid portfolioId, CancellationToken cancellationToken);
    Task<PagedResponse<PortfolioDto>> GetAllAsync(QueryFilter filter, CancellationToken cancellationToken);
    public Task<int> GetCountForUserAsync(string userId, CancellationToken cancellationToken);
    public Task<bool> NameExistsAsync(string userId, string portfolioName, CancellationToken cancellationToken);
    public Task<bool> NameExistsAsync(string userId, Guid portfolioId, string portfolioName, CancellationToken cancellationToken);
    public new Task<Portfolio?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    public Task<IReadOnlyList<Portfolio>> GetForUserAsync(string userId, CancellationToken cancellationToken);
    public Task<Portfolio?> GetWithStockProfilesByIdAsync(Guid id, CancellationToken cancellationToken);
    public Task<Portfolio?> GetOwnershipInfoAsync(Guid portfolioId, CancellationToken cancellationToken);
}