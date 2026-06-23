using Microsoft.AspNetCore.Identity;
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

    public async Task HandleAsync(UpdateRolesCommand request)
    {
        var isCurrentUserAdmin = await _userContext.IsInRoleAsync("Admin");
        if (!isCurrentUserAdmin)
        {
            throw new UnableToSetRoleException("You do not have permission to change the role");
        }

        var currentUserId = await _userContext.GetCurrentUserIdAsync();
        if (currentUserId == request.userId)
        {
            throw new UnableToSetRoleException("You cannot modify your own roles");
        }
        
        var rolesToAdd = request.newRoles
            .Where(role => role is "Admin" or "Manager")
            .ToArray();
        
        var result = await _identityService.UpdateUserRoles(request.userId, rolesToAdd);

        if (!result)
        {
            throw new UnableToSetRoleException("An error occured while updating the roles");
        }
    }
}