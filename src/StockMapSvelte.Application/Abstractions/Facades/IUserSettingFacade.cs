using StockMapSvelte.Application.UseCases.UserSettingUseCases.Commands;

namespace StockMapSvelte.Application.Abstractions.Facades;

public interface IUserSettingFacade
{
    Task SetPortfolioAsDefaultAsync(SetPortfolioAsDefaultCommand cmd);
    Task<Guid> GetDefaultPortfolioIdAsync();
    Task<bool> GetToastAutoHideValueAsync();
    Task<bool> SetToastAutoHideValueAsync(bool value);
    Task<int> GetToastAutoHideDelayMs();
    Task SetToastAutoHideDelayMsAsync(SetToastAutoHideDelayMsCommand cmd);
}