using StockMapSvelte.Application.Abstractions.Repositories;

namespace StockMapSvelte.Application.Abstractions;

public interface IUnitOfWork : IDisposable
{
    IPortfolioRepository Portfolios { get; }
    IStockProfileRepository StockProfiles { get; }
    IStockRepository Stocks { get; }
    IUserSettingRepository UserSettings { get; }
    
    Task<int> CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync();
}