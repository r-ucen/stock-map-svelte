using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StockMapSvelte.Api.Controllers;

[ApiController]
[Route("roles")]
public class RoleControler : Controller
{
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [Route("is-admin")]
    public IActionResult Index()
    {
        return NoContent();
    }
    
    [HttpGet]
    [Authorize(Roles = "Manager")]
    [Route("is-manager")]
    public IActionResult IsManager()
    {
        return NoContent();
    }
    
    [HttpGet]
    [Authorize(Policy = "IsCustomer")]
    [Route("is-customer")]
    public IActionResult IsCustomer()
    {
        return NoContent();
    }
}