using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Facades;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.DTOs.Common;
using StockMapSvelte.Application.UseCases.UserUseCases.Commands;
using StockMapSvelte.Application.UseCases.UserUseCases.Handlers;
using StockMapSvelte.Application.UseCases.UserUseCases.Queries;

namespace StockMapSvelte.Application.Facade;

public class UserFacade : IUserFacade
{
    private readonly DeleteUserHandler _deleteUserHandler;
    private readonly GetAllUsersQueriedHandler _getAllUsersQueriedHandler;
    private readonly GetUserHandler _getUserHandler;
    private readonly  IIdentityService _identityService;
    private readonly UpdateRolesHandler _updateRolesHandler;
    
    public UserFacade(
        DeleteUserHandler deleteUserHandler,
        GetAllUsersQueriedHandler getAllUsersQueriedHandler,
        GetUserHandler getUserHandler,
        IIdentityService identityService,
        UpdateRolesHandler updateRolesHandler)
    {
        _deleteUserHandler = deleteUserHandler;
        _getAllUsersQueriedHandler = getAllUsersQueriedHandler;
        _getUserHandler = getUserHandler;
        _identityService = identityService;
        _updateRolesHandler = updateRolesHandler;
    }

    public async Task LogOutAsync()
    {
         await _identityService.LogOutAsync();
    }

    public async Task UpdateRolesAsync(UpdateRolesCommand request)
    {
        await _updateRolesHandler.Handle(request);
    }

    public async Task DeleteUserAsync(DeleteUserCommand cmd, CancellationToken cancellationToken)
    {
        await _deleteUserHandler.Handle(cmd, cancellationToken);
    }
    
    public async Task<PagedResponse<UserDto>> GetAllUsersQueriedAsync(QueryFilter filter, CancellationToken cancellationToken)
    {
        return await _getAllUsersQueriedHandler.Handle(filter, cancellationToken);
    }
    
    public async Task<UserDto> GetUserAsync(GetUserQuery query, CancellationToken cancellationToken)
    {
        return await _getUserHandler.Handle(query, cancellationToken);
    }
}