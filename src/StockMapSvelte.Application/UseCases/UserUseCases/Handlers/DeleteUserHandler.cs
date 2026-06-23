using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.Exceptions.User;
using StockMapSvelte.Application.UseCases.UserUseCases.Commands;

namespace StockMapSvelte.Application.UseCases.UserUseCases.Handlers;

public class DeleteUserHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IUserContext _userContext;
    private readonly IIdentityService  _identityService;

    public DeleteUserHandler(IUserRepository userRepository, IUserContext userContext, IIdentityService identityService)
    {
        _userRepository = userRepository;
        _userContext = userContext;
        _identityService = identityService;
    }
    
    public async Task Handle(DeleteUserCommand cmd, CancellationToken cancellationToken)
    {
        // add check that prevents deleting the last admin in case delete permissions change in the future
        if (await _identityService.IsUserTheLastAdminAsync(cmd.UserId))
        {
            throw new DeleteUserFailException("Cannot delete the last admin");
        }
        
        var currentUserId = await _userContext.GetCurrentUserIdAsync();
        if (currentUserId == cmd.UserId)
        {
            throw new DeleteYourselfNotPossibleException("Cannot delete yourself.");
        }

        var result = await _userRepository.DeleteAsync(cmd.UserId, cancellationToken);
        
        if (!result)
        {
            throw new DeleteUserFailException($"Failed to delete user with id: {cmd.UserId}");
        }
    }
}