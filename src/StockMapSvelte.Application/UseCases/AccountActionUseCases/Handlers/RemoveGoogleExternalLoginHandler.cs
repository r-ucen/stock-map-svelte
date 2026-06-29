using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Exceptions.Account;
using StockMapSvelte.Application.UseCases.AccountActionUseCases.Commands;

namespace StockMapSvelte.Application.UseCases.AccountActionUseCases.Handlers;

public class RemoveGoogleExternalLoginHandler
{
    private readonly IIdentityService _identityService;

    public RemoveGoogleExternalLoginHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task Handle(RemoveGoogleExternalLoginCommand cmd)
    {
        if (!await _identityService.UserExistsAsync(cmd.UserId))
        {
            throw new RemoveGoogleExternalLoginFailed("User does not exist");
        }
        
        var hasPasswordConfigured = await _identityService.HasPasswordConfiguredAsync(cmd.UserId);
        if (!hasPasswordConfigured)
        {
            throw new RemoveGoogleExternalLoginFailed("Cannot remove Google login because no password is set. " +
                                                      "Please set a password before removing the Google login.");
        }

        var googleLogin = await _identityService.GetGoogleLoginAsync(cmd.UserId);
        if (googleLogin == null)
        {
            throw new RemoveGoogleExternalLoginFailed("Google login not found.");
        }
        
        var result = await _identityService.RemoveGoogleExternalLoginAsync(cmd.UserId, googleLogin);
        if (!result)
        {
            throw new RemoveGoogleExternalLoginFailed("Failed to remove Google external login. Please try again later.");
        }
    }
}