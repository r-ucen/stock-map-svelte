using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.DTOs.Common;

namespace StockMapSvelte.Application.UseCases.UserUseCases.Handlers;

public class GetAllUsersQueriedHandler
{
    private readonly IUserRepository _userRepository;

    public GetAllUsersQueriedHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<PagedResponse<UserDto>> HandleAsync(QueryFilter filter, CancellationToken cancellationToken)
    {
        return await _userRepository.GetAllAsyncQueried(filter, cancellationToken);
    }
}