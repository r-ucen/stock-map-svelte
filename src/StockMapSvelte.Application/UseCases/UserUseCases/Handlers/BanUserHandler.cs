using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Exceptions.User;
using StockMapSvelte.Application.UseCases.UserUseCases.Commands;

namespace StockMapSvelte.Application.UseCases.UserUseCases.Handlers;

public class BanUserHandler
{
    private readonly IIdentityService _identityService;
    private readonly IUserContext _userContext;

    public BanUserHandler(IIdentityService identityService, IUserContext userContext)
    {
        _identityService = identityService;
        _userContext = userContext;
    }

    public async Task Handle(BanUserCommand cmd)
    {
        if (!await _identityService.UserExistsAsync(cmd.UserId))
        { throw new UserNotFoundException($"User with id {cmd.UserId} not found."); }
        
        if (await _identityService.IsUserLockedOutAsync(cmd.UserId))
        { throw new UserAlreadyBannedException($"User with id {cmd.UserId} is already banned."); }
        
        var currentUserId = await _userContext.GetCurrentUserIdAsync();
        if (currentUserId == cmd.UserId)
        { throw new BanYourselfNotPossibleException("Cannot ban yourself."); }
        
        if (await _identityService.IsUserInRoleAsync(cmd.UserId, "Admin"))
        { throw new BanUserNotAllowed("Cannot ban an admin user."); }
        
        var result = await _identityService.LockOutAsync(cmd.UserId);

        if (!result)
        { throw new BanUserFailException("Failed to ban user."); }
    }
}