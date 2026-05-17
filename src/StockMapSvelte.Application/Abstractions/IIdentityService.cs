using StockMapSvelte.Application.DTOs;

namespace StockMapSvelte.Application.Abstractions;

public interface IIdentityService
{
    Task LogOutAsync();
    Task<ExternalLoginResponse> HandleExternalLoginAsync();
    Task<bool> LinkAccountWithPasswordAsync(string email, string password);
}