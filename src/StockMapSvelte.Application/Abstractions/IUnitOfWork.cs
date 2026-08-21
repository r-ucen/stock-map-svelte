using StockMapSvelte.Application.Abstractions.Repositories;

namespace StockMapSvelte.Application.Abstractions;

public interface IUnitOfWork : IDisposable
{
    IPortfolioRepository Portfolios { get; }
    IStockProfileRepository StockProfile { get; }
    IStockRepository Stocks { get; }
    ITreeMapRepository TreeMaps { get; }
    IUserRepository Users { get; }
    IUserSettingRepository UserSettings { get; }
    
    Task<int> CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync();
}