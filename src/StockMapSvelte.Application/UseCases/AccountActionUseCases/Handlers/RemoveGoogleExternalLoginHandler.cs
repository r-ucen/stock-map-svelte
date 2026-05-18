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

    public async Task Handle(RemoveGoogleExternalLoginCommand command)
    {
        if (string.IsNullOrEmpty(command.UserId))
        {
            throw new ArgumentException("User ID must be provided to remove google external login.");
        }
        
        var result = await _identityService.RemoveGoogleExternalLoginAsync(command.UserId);

        if (!result.Success)
        {
            throw new RemoveGoogleExternalLoginFailed(result.Message ?? "Failed to remove Google external login. Please try again later.");
        }
    }
}