using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.DTOs.Common;
using StockMapSvelte.Application.UseCases.UserUseCases.Commands;
using StockMapSvelte.Application.UseCases.UserUseCases.Queries;

namespace StockMapSvelte.Application.Abstractions.Facades;

public interface IUserFacade
{
    Task DeleteUserAsync(DeleteUserCommand cmd, CancellationToken cancellationToken);
    Task<PagedResponse<UserDto>> GetAllUsersQueriedAsync(QueryFilter filter, CancellationToken cancellationToken);
    Task<UserDto> GetUserAsync(GetUserQuery query, CancellationToken cancellationToken);
    Task LogOutAsync();
    Task UpdateRolesAsync(UpdateRolesCommand cmd);
    public Task BanUserAsync(BanUserCommand cmd);
    public Task UnbanUserAsync(UnbanUserCommand cmd);
}