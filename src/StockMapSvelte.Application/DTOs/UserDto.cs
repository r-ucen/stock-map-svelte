namespace StockMapSvelte.Application.DTOs;

public record UserDto(string Id, string Email, string UserName, IList<string> Roles, bool IsBanned);