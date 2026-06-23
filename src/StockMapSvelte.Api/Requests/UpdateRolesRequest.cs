namespace StockMapSvelte.Api.Requests;

public record UpdateRolesRequest(string userId, string[] newRoles);