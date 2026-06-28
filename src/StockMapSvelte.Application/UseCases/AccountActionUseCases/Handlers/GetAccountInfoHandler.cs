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
        var hasPassword = await _identityService.HasPasswordConfiguredAsync(query.UserId);
        var externalLogins = await _identityService.GetExternalLoginsAsync(query.UserId);
        var isAdmin = await _identityService.IsUserInRoleAsync(query.UserId, "Admin");
        var isManager = await _identityService.IsUserInRoleAsync(query.UserId, "Manager");
        var isCustomerPolicy = await _identityService.DoesUserComplyWithPolicyAsync(query.UserId, "IsCustomer");
        var email = await _identityService.GetUserEmailAsync(query.UserId);
        var isEmailConfirmed = await _identityService.IsEmailConfirmedAsync(query.UserId);

        if (email == null) { throw new ArgumentException($"No email address found for user with an id: {query.UserId}."); }

        return new AccountInfoDto
        (
            hasPassword,
            externalLogins.Any(),
            externalLogins.Select(x => new UserLoginInfoDto
            (
                x.LoginProvider,
                x.ProviderDisplayName
            )).ToList(),
            isAdmin,
            isManager,
            isCustomerPolicy,
            email,
            isEmailConfirmed
        );
    }
}