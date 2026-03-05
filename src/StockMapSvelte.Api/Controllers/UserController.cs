using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockMapSvelte.Application.Abstractions.Facades;
using StockMapSvelte.Application.UseCases.UserUseCases.Queries;

namespace StockMapSvelte.Api.Controllers;

[ApiController]
[Authorize]
[Route("user")]
public class UserController : Controller
{
    private readonly IUserFacade _userFacade;
    
    public UserController(IUserFacade userFacade)
    {
        _userFacade = userFacade;
    }
    
    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    [Route("all")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userFacade.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    [Route("{userId}")]
    public async Task<IActionResult> GetUser(string userId)
    {
        var query = new GetUserQuery
        {
            UserId = userId
        };
        
        var user = await _userFacade.GetUserAsync(query);
        return Ok(user);
    }
    
    
}