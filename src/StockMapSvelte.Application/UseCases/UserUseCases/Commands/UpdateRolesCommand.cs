namespace StockMapSvelte.Application.UseCases.UserUseCases.Commands;

public record UpdateRolesCommand(string userId, string[] newRoles);