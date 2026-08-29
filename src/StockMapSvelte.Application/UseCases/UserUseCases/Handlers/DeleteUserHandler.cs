using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Exceptions.User;
using StockMapSvelte.Application.UseCases.UserUseCases.Commands;

namespace StockMapSvelte.Application.UseCases.UserUseCases.Handlers;

public class DeleteUserHandler
{
    private readonly IUserContext _userContext;
    private readonly IIdentityService  _identityService;

    public DeleteUserHandler(IUserContext userContext, IIdentityService identityService)
    {
        _userContext = userContext;
        _identityService = identityService;
    }
    
    public async Task Handle(DeleteUserCommand cmd, CancellationToken cancellationToken)
    {
        if (!await _identityService.UserExistsAsync(cmd.UserId))
        { throw new DeleteUserFailException($"User with id {cmd.UserId} not found."); }
        
        // add check that prevents deleting the last admin in case delete permissions change in the future
        if (await _identityService.IsUserTheLastAdminAsync(cmd.UserId))
        { throw new DeleteUserFailException("Cannot delete the last admin"); }
        
        var currentUserId = await _userContext.GetCurrentUserIdAsync();
        if (currentUserId == cmd.UserId)
        { throw new DeleteYourselfNotPossibleException("Cannot delete yourself."); }

        var result = await _identityService.DeleteUserAsync(cmd.UserId, cancellationToken);
        if (!result) { throw new DeleteUserFailException($"Failed to delete user with id: {cmd.UserId}"); }
    }
}