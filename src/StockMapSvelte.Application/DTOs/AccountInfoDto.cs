namespace StockMapSvelte.Application.DTOs;

public class AccountInfoDto
{
    public bool HasPasswordConfigured { get; set; }
    public bool HasExternalLoginConfigured { get; set; }
    public IList<UserLoginInfoDto>? ExternalLogins { get; set; }
}