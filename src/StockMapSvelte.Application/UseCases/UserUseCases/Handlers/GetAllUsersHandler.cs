using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.DTOs;

namespace StockMapSvelte.Application.UseCases.UserUseCases.Handlers;

public class GetAllUsersHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IUserContext _userContext;

    public GetAllUsersHandler(IUserRepository userRepository, IUserContext userContext)
    {
        _userRepository = userRepository;
        _userContext = userContext;
    }
    
    public async Task<List<UserDto>> HandleAsync()
    {
        if (!await _userContext.IsInRoleAsync("Admin") && !await _userContext.IsInRoleAsync("Manager"))
        {
            throw new UnauthorizedAccessException($"User {await _userContext.GetCurrentUserIdAsync()} does not have permission to get all users.");
        }
        return await _userRepository.GetAllAsync();
    }
}