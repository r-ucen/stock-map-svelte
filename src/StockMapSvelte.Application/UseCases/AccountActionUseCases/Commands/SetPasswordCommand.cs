namespace StockMapSvelte.Application.UseCases.AccountActionUseCases.Commands;

public record SetPasswordCommand(string UserId, string NewPassword);