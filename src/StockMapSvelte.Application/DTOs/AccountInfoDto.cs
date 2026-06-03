namespace StockMapSvelte.Application.DTOs;

public record AccountInfoDto(
    bool HasPasswordConfigured,
    bool HasExternalLoginConfigured,
    IList<UserLoginInfoDto>? ExternalLogins
);