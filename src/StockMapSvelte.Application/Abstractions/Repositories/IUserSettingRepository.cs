namespace StockMapSvelte.Application.Abstractions.Repositories;

public interface IUserSettingRepository
{
    Task<int> SetPortfolioAsDefaultAsync(string userId, Guid portfolioId);
    Task<Guid?> GetDefaultPortfolioAsync(string userId);
    Task<int> SetToastAutoHideValueAsync(string userId, bool autoHide);
    Task<bool?> GetToastAutoHideValueAsync(string userId);
    Task<int?> GetToastAutoHideDelayMs(string userId);
    Task<int> SetToastAutoHideDelayMsAsync(string userId, int delayMs);
}