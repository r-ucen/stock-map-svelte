using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.UseCases.UserUseCases.Queries;
using StockMapSvelte.Application.DTOs;

namespace StockMapSvelte.Application.UseCases.UserUseCases.Handlers;

public class GetUserHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IUserContext _userContext;

    public GetUserHandler(IUserRepository userRepository, IUserContext userContext)
    {
        _userRepository = userRepository;
        _userContext = userContext;
    }
    
    public async Task<UserDto> HandleAsync(GetUserQuery query)
    {
        if (!await _userContext.IsInRoleAsync("Admin"))
        {
            throw new UnauthorizedAccessException($"User {await _userContext.GetCurrentUserIdAsync()} does not have permission to get this user.");
        }
        return await _userRepository.GetByIdAsync(query.UserId);
    }
}