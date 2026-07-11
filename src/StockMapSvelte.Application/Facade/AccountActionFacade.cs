using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Facades;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.UseCases.AccountActionUseCases.Commands;
using StockMapSvelte.Application.UseCases.AccountActionUseCases.Handlers;
using StockMapSvelte.Application.UseCases.AccountActionUseCases.Queries;

namespace StockMapSvelte.Application.Facade;

public class AccountActionFacade : IAccountActionFacade
{
    private readonly GetAccountInfoHandler _getAccountInfoHandler;
    private readonly DeleteAccountHandler _deleteAccountHandler;
    private readonly SetPasswordHandler _setPasswordHandler;
    private readonly RemoveGoogleExternalLoginHandler _removeGoogleExternalLoginHandler;
    
    public AccountActionFacade(
        GetAccountInfoHandler getAccountInfoHandler,
        DeleteAccountHandler deleteAccountHandler,
        SetPasswordHandler setPasswordHandler,
        RemoveGoogleExternalLoginHandler removeGoogleExternalLoginHandler)
    {
        _getAccountInfoHandler = getAccountInfoHandler;
        _deleteAccountHandler = deleteAccountHandler;
        _setPasswordHandler = setPasswordHandler;
        _removeGoogleExternalLoginHandler = removeGoogleExternalLoginHandler;
    }

    public async Task<AccountInfoDto> GetAccountInfoAsync(GetAccountInfoQuery query)
    {
        return await _getAccountInfoHandler.Handle(query);
    }

    public async Task DeleteMyAccountAsync(DeleteAccountCommand command)
    {
        await _deleteAccountHandler.Handle(command);
    }

    public async Task SetPasswordAsync(SetPasswordCommand command)
    {
        await _setPasswordHandler.Handle(command);
    }

    public async Task RemoveGoogleExternalLoginAsync(RemoveGoogleExternalLoginCommand command)
    {
        await _removeGoogleExternalLoginHandler.Handle(command);
    }
}