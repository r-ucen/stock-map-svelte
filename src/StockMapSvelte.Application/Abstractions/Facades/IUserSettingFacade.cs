using StockMapSvelte.Application.UseCases.UserSettingUseCases.Commands;

namespace StockMapSvelte.Application.Abstractions.Facades;

public interface IUserSettingFacade
{
    Task SetPortfolioAsDefaultAsync(SetPortfolioAsDefaultCommand cmd, CancellationToken cancellationToken);
    Task<Guid> GetDefaultPortfolioIdAsync(CancellationToken cancellationToken);
    Task<bool> GetToastAutoHideValueAsync();
    Task SetToastAutoHideValueAsync(SetToastAutoHideValueCommand cmd);
    Task<int> GetToastAutoHideDelayMs();
    Task SetToastAutoHideDelayMsAsync(SetToastAutoHideDelayMsCommand cmd);
}