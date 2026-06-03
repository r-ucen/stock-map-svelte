using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.UseCases.AccountActionUseCases.Queries;

namespace StockMapSvelte.Application.UseCases.AccountActionUseCases.Handlers;

public class GetAccountInfoHandler
{
    private readonly IIdentityService _identityService;
    
    public GetAccountInfoHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<AccountInfoDto> Handle(GetAccountInfoQuery query)
    {
        if (string.IsNullOrEmpty(query.UserId))
        {
            throw new ArgumentException("User ID must be provided to fetch account info.");
        }
        
        var hasPassword = await _identityService.HasPasswordConfiguredAsync(query.UserId);
        var externalLogins = await _identityService.GetExternalLoginsAsync(query.UserId);
        

        return new AccountInfoDto
        (
            hasPassword,
            externalLogins.Any(),
            externalLogins.Select(x => new UserLoginInfoDto
            (
                x.LoginProvider,
                x.ProviderDisplayName
            )).ToList()
        );
    }
}