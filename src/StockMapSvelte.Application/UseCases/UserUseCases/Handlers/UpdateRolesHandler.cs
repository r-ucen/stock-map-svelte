using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Exceptions.User;
using StockMapSvelte.Application.UseCases.UserUseCases.Commands;

namespace StockMapSvelte.Application.UseCases.UserUseCases.Handlers;

public class UpdateRolesHandler
{
    private readonly IUserContext _userContext;
    private readonly IIdentityService _identityService;
    
    public UpdateRolesHandler(IUserContext userContext, IIdentityService identityService)
    {
        _userContext = userContext;
        _identityService = identityService;
    }

    public async Task Handle(UpdateRolesCommand cmd)
    {
        var currentUserId = await _userContext.GetCurrentUserIdAsync();
        if (currentUserId == cmd.UserId) { throw new UnableToSetRoleException("You cannot modify your own roles"); }
        
        var currentRoles = await _identityService.GetUserRolesAsync(cmd.UserId);
        
        var isRemovingAdmin = currentRoles.Contains("Admin") && !cmd.NewRoles.Contains("Admin");
        if (isRemovingAdmin && await _identityService.IsUserTheLastAdminAsync(cmd.UserId))
        { throw new UnableToSetRoleException("Cannot remove the last admin"); }
        
        var result = await _identityService.UpdateUserRoles(cmd.UserId, cmd.NewRoles);
        if (!result) { throw new UnableToSetRoleException("An error occured while updating the roles"); }
    }
}