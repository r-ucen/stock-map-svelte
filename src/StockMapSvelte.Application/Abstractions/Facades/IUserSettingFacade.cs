namespace StockMapSvelte.Application.Abstractions.Facades;

public interface IUserSettingFacade
{
    Task<bool> SetPortfolioAsDefaultAsync(Guid portfolioId);
    Task<Guid> GetDefaultPortfolioIdAsync();
    Task<bool> GetToastAutoHideValueAsync();
    Task<bool> SetToastAutoHideValueAsync(bool value);
    Task<int> GetToastAutoHideDelayMs();
    Task<bool> SetToastAutoHideDelayMsAsync(int value);
}