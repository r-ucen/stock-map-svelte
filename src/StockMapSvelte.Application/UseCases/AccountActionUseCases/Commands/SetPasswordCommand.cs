namespace StockMapSvelte.Application.UseCases.AccountActionUseCases.Commands;

public class SetPasswordCommand
{
    public string UserId { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}