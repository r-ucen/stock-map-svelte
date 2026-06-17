namespace StockMapSvelte.Application.DTOs;

public record AccountInfoDto(
    bool HasPasswordConfigured,
    bool HasExternalLoginConfigured,
    IList<UserLoginInfoDto>? ExternalLogins,
    bool IsAdmin,
    bool IsManager,
    bool IsCustomer,
    string Email,
    bool IsEmailConfirmed
);