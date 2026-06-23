using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Facades;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.UseCases.UserUseCases.Commands;
using StockMapSvelte.Application.UseCases.UserUseCases.Handlers;
using StockMapSvelte.Application.UseCases.UserUseCases.Queries;

namespace StockMapSvelte.Application.Facade;

public class UserFacade : IUserFacade
{
    private readonly DeleteUserHandler _deleteUserHandler;
    private readonly GetAllUsersHandler _getAllUsersHandler;
    private readonly GetUserHandler _getUserHandler;
    private readonly  IIdentityService _identityService;
    private readonly UpdateRolesHandler _updateRolesHandler;
    
    public UserFacade(
        DeleteUserHandler deleteUserHandler,
        GetAllUsersHandler handler,
        GetUserHandler getUserHandler,
        IIdentityService identityService,
        UpdateRolesHandler updateRolesHandler)
    {
        _deleteUserHandler = deleteUserHandler;
        _getAllUsersHandler = handler;
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
        await _updateRolesHandler.HandleAsync(request);
    }

    public async Task DeleteUserAsync(DeleteUserCommand cmd, CancellationToken cancellationToken)
    {
        await _deleteUserHandler.Handle(cmd, cancellationToken);
    }
    
    public async Task<List<UserDto>> GetAllUsersAsync(CancellationToken cancellationToken)
    {
        return await _getAllUsersHandler.HandleAsync(cancellationToken);
    }
    
    public async Task<UserDto> GetUserAsync(GetUserQuery query, CancellationToken cancellationToken)
    {
        return await _getUserHandler.HandleAsync(query, cancellationToken);
    }
}