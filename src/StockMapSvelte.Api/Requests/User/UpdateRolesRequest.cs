namespace StockMapSvelte.Api.Requests.User;

public record UpdateRolesRequest(string UserId, string[] NewRoles);