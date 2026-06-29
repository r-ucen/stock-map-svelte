using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Exceptions.Account;
using StockMapSvelte.Application.UseCases.AccountActionUseCases.Commands;

namespace StockMapSvelte.Application.UseCases.AccountActionUseCases.Handlers;

public class DeleteAccountHandler
{
    private readonly IIdentityService _identityService;
    
    public DeleteAccountHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }
    
    public async Task Handle(DeleteAccountCommand request)
    {
        var result = await _identityService.DeleteAccountAsync(request.UserId);
        
        if (!result) { throw new DeleteAccountFailedException("Failed to delete account. Please try again later."); }
    }
}