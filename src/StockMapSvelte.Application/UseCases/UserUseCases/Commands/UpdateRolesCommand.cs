namespace StockMapSvelte.Application.UseCases.UserUseCases.Commands;

public record UpdateRolesCommand(string UserId, string[] NewRoles);