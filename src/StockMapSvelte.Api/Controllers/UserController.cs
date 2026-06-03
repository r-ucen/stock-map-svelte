using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using StockMapSvelte.Application.Abstractions.Facades;
using StockMapSvelte.Application.UseCases.UserUseCases.Commands;
using StockMapSvelte.Application.UseCases.UserUseCases.Queries;

namespace StockMapSvelte.Api.Controllers;

[EnableRateLimiting("DataPolicy")]
[ApiController]
[Authorize]
[Route("users")]
public class UserController : Controller
{
    private readonly IUserFacade _userFacade;
    
    public UserController(IUserFacade userFacade)
    {
        _userFacade = userFacade;
    }
    
    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken = default)
    {
        var users = await _userFacade.GetAllUsersAsync(cancellationToken);
        return Ok(users);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    [Route("{userId}")]
    public async Task<IActionResult> GetUser(string userId, CancellationToken cancellationToken = default)
    {
        var query = new GetUserQuery(userId);
        
        var user = await _userFacade.GetUserAsync(query, cancellationToken);
        return Ok(user);
    }

    [HttpDelete]
    [Authorize(Roles = "Admin")]
    [Route("{userId}")]
    public async Task<IActionResult> DeleteUser(string userId, CancellationToken cancellationToken)
    {
        var cmd = new DeleteUserCommand(userId);
        
        await _userFacade.DeleteUserAsync(cmd, cancellationToken);
        return NoContent();
    }
}