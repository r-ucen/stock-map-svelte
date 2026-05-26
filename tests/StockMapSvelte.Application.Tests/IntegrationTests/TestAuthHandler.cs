using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace StockMapSvelte.Application.Tests.IntegrationTests;

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string AuthenticationScheme = "Test";
    public const string RoleHeaderName = "X-Test-Role";
    
    public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (Request.Headers.TryGetValue(RoleHeaderName, out var roleValues))
        {
            var requestedRole = roleValues.ToString();
            
            if (requestedRole.Equals("Anonymous", StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(AuthenticateResult.Fail("Anonymous client"));
            }
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, requestedRole)
            };

            var identity = new ClaimsIdentity(claims, AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, AuthenticationScheme);

            var result = AuthenticateResult.Success(ticket);
            return Task.FromResult(result);
        }
        
        var defaultClaims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, "Customer")
        };
        
        var defaultIdentity = new ClaimsIdentity(defaultClaims, AuthenticationScheme);
        var defaultPrincipal = new ClaimsPrincipal(defaultIdentity);
        var defaultTicket = new AuthenticationTicket(defaultPrincipal, AuthenticationScheme);

        var defaultResult = AuthenticateResult.Success(defaultTicket);
        return Task.FromResult(defaultResult);
    }
}