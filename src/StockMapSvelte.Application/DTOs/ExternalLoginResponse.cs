using StockMapSvelte.Application.Enums;

namespace StockMapSvelte.Application.DTOs;

public class ExternalLoginResponse
{
    public ExternalLoginResult Result { get; set; }
    public string? Email { get; set; }
}