using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Exceptions.User;
using StockMapSvelte.Application.UseCases.UserUseCases.Commands;

namespace StockMapSvelte.Application.UseCases.UserUseCases.Handlers;

public class UnbanUserHandler
{
    private readonly IIdentityService _identityService;
    private readonly IUserContext _userContext;

    public UnbanUserHandler(IIdentityService identityService, IUserContext userContext)
    {
        _identityService = identityService;
        _userContext = userContext;
    }

    public async Task Handle(UnbanUserCommand cmd)
    {
        if (!await _identityService.UserExistsAsync(cmd.UserId))
        { throw new UserNotFoundException($"User with id {cmd.UserId} not found."); }
        
        var currentUserId = await _userContext.GetCurrentUserIdAsync();
        if (currentUserId == cmd.UserId)
        { throw new UnbanYourselfNotPossibleException("Cannot unban yourself."); }
        
        if (!await _identityService.IsUserLockedOutAsync(cmd.UserId))
        { throw new UserNotBannedException($"User with id {cmd.UserId} is not banned."); }
        
        if (await _identityService.IsUserInRoleAsync(cmd.UserId, "Admin"))
        { throw new UnbanUserNotAllowed("Cannot unban an admin user."); }
        
        var result = await _identityService.UnlockAsync(cmd.UserId);

        if (!result)
        { throw new UnbanUserFailException("Failed to unban the user."); }
    }
}