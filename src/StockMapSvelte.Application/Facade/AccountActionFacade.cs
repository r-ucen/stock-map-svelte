using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Facades;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.UseCases.AccountActionUseCases.Commands;
using StockMapSvelte.Application.UseCases.AccountActionUseCases.Handlers;
using StockMapSvelte.Application.UseCases.AccountActionUseCases.Queries;

namespace StockMapSvelte.Application.Facade;

public class AccountActionFacade : IAccountActionFacade
{
    private readonly IUserContext _userContext;
    
    private readonly GetAccountInfoHandler _getAccountInfoHandler;
    private readonly DeleteAccountHandler _deleteAccountHandler;
    private readonly SetPasswordHandler _setPasswordHandler;
    private readonly RemoveGoogleExternalLoginHandler _removeGoogleExternalLoginHandler;
    
    public AccountActionFacade(
        IUserContext userContext,
        GetAccountInfoHandler getAccountInfoHandler,
        DeleteAccountHandler deleteAccountHandler,
        SetPasswordHandler setPasswordHandler,
        RemoveGoogleExternalLoginHandler removeGoogleExternalLoginHandler)
    {
        _userContext = userContext;
        _getAccountInfoHandler = getAccountInfoHandler;
        _deleteAccountHandler = deleteAccountHandler;
        _setPasswordHandler = setPasswordHandler;
        _removeGoogleExternalLoginHandler = removeGoogleExternalLoginHandler;
    }

    public async Task<AccountInfoDto> GetAccountInfoAsync()
    {
        var currentUserId = await _userContext.GetCurrentUserIdAsync();

        var query = new GetAccountInfoQuery(currentUserId);
        
        return await _getAccountInfoHandler.Handle(query);
    }

    public async Task DeleteMyAccountAsync()
    {
        var currentUserId = await _userContext.GetCurrentUserIdAsync();

        var query = new DeleteAccountCommand(currentUserId);
        
        await _deleteAccountHandler.Handle(query);
    }

    public async Task SetPasswordAsync(string newPassword)
    {
        var currentUserId = await _userContext.GetCurrentUserIdAsync();

        var command = new SetPasswordCommand(currentUserId, newPassword);
        
        await _setPasswordHandler.Handle(command);
    }

    public async Task RemoveGoogleExternalLoginAsync()
    {
        var currentUserId = await _userContext.GetCurrentUserIdAsync();

        var command = new RemoveGoogleExternalLoginCommand(currentUserId);
        
        await _removeGoogleExternalLoginHandler.Handle(command);
    }
}