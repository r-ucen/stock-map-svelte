using StockMapSvelte.Application.Enums;

namespace StockMapSvelte.Application.DTOs;

public record ExternalLoginResponse(ExternalLoginResult Result, string? Email);