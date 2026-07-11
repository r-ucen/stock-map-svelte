using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using StockMapSvelte.Api.Requests.OAuth;
using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Infrastructure.Identity;
using StockMapSvelte.Application.Enums;

namespace StockMapSvelte.Api.Controllers;

[EnableRateLimiting("DataPolicy")]
[ApiController]
[Route("oauth")]
public class OAuthController : Controller
{
    private readonly IIdentityService _identityService;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly string _frontendBaseUrl;

    public OAuthController(
        SignInManager<ApplicationUser> signInManager,
        IIdentityService identityService,
        IConfiguration configuration)
    {
        _signInManager = signInManager;
        _identityService = identityService;
        _frontendBaseUrl= configuration["FrontendUrl"] ?? "https://localhost:5173";
    }

    [HttpGet("google-login")]
    public IActionResult GoogleLogin(string? returnUrl = null)
    {
        var redirectUrl = Url.Action(nameof(GoogleResponse), "OAuth", new { returnUrl }, Request.Scheme);

        const string scheme = "GoogleOpenIdConnect";
        
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(
            scheme, redirectUrl);
        
        return Challenge(properties, scheme);
    }

    [HttpGet("google-response")]
    public async Task<IActionResult> GoogleResponse(string? returnUrl = null)
    {
        var response = await _identityService.HandleExternalLoginAsync();

        return response.Result switch
        {
            ExternalLoginResult.Success => Redirect($"{returnUrl ?? _frontendBaseUrl}?login=true"),
            ExternalLoginResult.AccountExistsRequireLinking => Redirect($"{_frontendBaseUrl}/link-account?email={Uri.EscapeDataString(response.Email!)}"),
            _ => Redirect($"{_frontendBaseUrl}/oauth-failed")
        };
    }
    
    [HttpPost("link-oauth-confirm")]
    public async Task<IActionResult> LinkConfirm([FromBody] LinkRequest request)
    {
        var success = await _identityService.LinkAccountWithPasswordAsync(request.Email, request.Password);
        return success ? Ok() : BadRequest("Invalid credentials");
    }
}