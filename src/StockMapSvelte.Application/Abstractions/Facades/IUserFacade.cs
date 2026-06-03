using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.UseCases.UserUseCases.Commands;
using StockMapSvelte.Application.UseCases.UserUseCases.Queries;

namespace StockMapSvelte.Application.Abstractions.Facades;

public interface IUserFacade
{
    Task DeleteUserAsync(DeleteUserCommand cmd, CancellationToken cancellationToken);
    Task<List<UserDto>> GetAllUsersAsync(CancellationToken cancellationToken);
    Task<UserDto> GetUserAsync(GetUserQuery query, CancellationToken cancellationToken);
    Task LogOutAsync();
}