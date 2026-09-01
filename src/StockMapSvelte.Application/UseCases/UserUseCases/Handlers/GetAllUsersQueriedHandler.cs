using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.DTOs.Common;

namespace StockMapSvelte.Application.UseCases.UserUseCases.Handlers;

public class GetAllUsersQueriedHandler
{
    private readonly IIdentityService  _identityService;

    public GetAllUsersQueriedHandler(IIdentityService identityService)
    {
        _identityService =  identityService;
    }
    
    public async Task<PagedResponse<UserDto>> Handle(QueryFilter filter, CancellationToken cancellationToken)
    {
        return await _identityService.GetAllUsersAsync(filter, cancellationToken);
    }
}