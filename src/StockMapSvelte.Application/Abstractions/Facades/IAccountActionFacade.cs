using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.UseCases.AccountActionUseCases.Queries;

namespace StockMapSvelte.Application.Abstractions.Facades;

public interface IAccountActionFacade
{
    public Task<AccountInfoDto> GetAccountInfoAsync();
    public Task DeleteMyAccountAsync();
    public Task SetPasswordAsync(string newPassword);
    public Task RemoveGoogleExternalLoginAsync();
}