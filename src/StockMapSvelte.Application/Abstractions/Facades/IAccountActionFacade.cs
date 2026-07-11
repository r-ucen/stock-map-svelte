using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.UseCases.AccountActionUseCases.Commands;
using StockMapSvelte.Application.UseCases.AccountActionUseCases.Queries;

namespace StockMapSvelte.Application.Abstractions.Facades;

public interface IAccountActionFacade
{
    public Task<AccountInfoDto> GetAccountInfoAsync(GetAccountInfoQuery query);
    public Task DeleteMyAccountAsync(DeleteAccountCommand command);
    public Task SetPasswordAsync(SetPasswordCommand command);
    public Task RemoveGoogleExternalLoginAsync(RemoveGoogleExternalLoginCommand command);
}