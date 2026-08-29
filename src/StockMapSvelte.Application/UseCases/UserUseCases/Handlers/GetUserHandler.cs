using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.UseCases.UserUseCases.Queries;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.Exceptions.User;

namespace StockMapSvelte.Application.UseCases.UserUseCases.Handlers;

public class GetUserHandler
{
    private readonly IIdentityService _identityService;

    public GetUserHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }
    
    public async Task<UserDto> Handle(GetUserQuery query, CancellationToken cancellationToken)
    {
        var user = await _identityService.GetUserByIdAsync(query.UserId, cancellationToken);
        return user ?? throw new UserNotFoundException("User not found");
    }
}