using StockMapSvelte.Domain.Entities;

namespace StockMapSvelte.Application.Abstractions.Repositories;

public interface IUserSettingRepository
{
    public Task<UserSetting?> GetAsync(string userId, CancellationToken cancellationToken = default);
    public Task<UserSetting> AddAsync(UserSetting entity, CancellationToken cancellationToken = default);
    Task<Guid?> GetDefaultPortfolioIdAsync(string userId, CancellationToken cancellationToken);
}