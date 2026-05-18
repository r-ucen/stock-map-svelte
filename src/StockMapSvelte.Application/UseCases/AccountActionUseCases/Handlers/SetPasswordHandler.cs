using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Exceptions.Account;
using StockMapSvelte.Application.UseCases.AccountActionUseCases.Commands;

namespace StockMapSvelte.Application.UseCases.AccountActionUseCases.Handlers;

public class SetPasswordHandler
{
    private readonly IIdentityService _identityService;
    
    public SetPasswordHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }
    
    public async Task Handle(SetPasswordCommand cmd)
    {
        if (string.IsNullOrEmpty(cmd.UserId))
        {
            throw new ArgumentException("User ID must be provided to set password.");
        }
        
        if (string.IsNullOrWhiteSpace(cmd.NewPassword))
        {
            throw new ArgumentException("New password must be provided.");
        }
        
        var result = await _identityService.SetAccountPasswordAsync(cmd.UserId, cmd.NewPassword);

        if (!result)
        {
            throw new SetPasswordFailedException();
        }
    }
}