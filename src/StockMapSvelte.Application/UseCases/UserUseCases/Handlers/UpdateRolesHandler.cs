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

    public async Task Handle(UpdateRolesCommand request)
    {
        var currentUserId = await _userContext.GetCurrentUserIdAsync();
        if (currentUserId == request.UserId) { throw new UnableToSetRoleException("You cannot modify your own roles"); }
        
        var currentRoles = await _identityService.GetUserRolesAsync(request.UserId);
        var isRemovingAdmin = currentRoles.Contains("Admin") && !request.NewRoles.Contains("Admin");
        if (isRemovingAdmin && await _identityService.IsUserTheLastAdminAsync(request.UserId))
        {
            throw new UnableToSetRoleException("Cannot remove the last admin");
        }
        
        var result = await _identityService.UpdateUserRoles(request.UserId, request.NewRoles);
        if (!result) { throw new UnableToSetRoleException("An error occured while updating the roles"); }
    }
}