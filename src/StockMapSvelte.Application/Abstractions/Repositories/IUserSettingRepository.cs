using StockMapSvelte.Domain.Entities;

namespace StockMapSvelte.Application.Abstractions.Repositories;

public interface IUserSettingRepository
{
    Task<int> SetPortfolioAsDefaultAsync(string userId, Guid portfolioId);
    
    Task<int> SetToastAutoHideValueAsync(string userId, bool autoHide);
    Task<bool?> GetToastAutoHideValueAsync(string userId);
    Task<int?> GetToastAutoHideDelayMs(string userId);
    Task<int> SetToastAutoHideDelayMsAsync(string userId, int delayMs);
    
    // REFACTORED

    public Task<UserSetting?> GetAsync(string userId, CancellationToken cancellationToken = default);
    public Task<UserSetting> AddAsync(UserSetting entity, CancellationToken cancellationToken = default);
    Task<Guid?> GetDefaultPortfolioIdAsync(string userId, CancellationToken cancellationToken);
}