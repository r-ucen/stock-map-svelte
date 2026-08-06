using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.UseCases.UserUseCases.Queries;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.Exceptions.User;

namespace StockMapSvelte.Application.UseCases.UserUseCases.Handlers;

public class GetUserHandler
{
    private readonly IUserRepository _userRepository;

    public GetUserHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<UserDto> Handle(GetUserQuery query, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(query.UserId, cancellationToken);
        return user ?? throw new UserNotFoundException("User not found");
    }
}