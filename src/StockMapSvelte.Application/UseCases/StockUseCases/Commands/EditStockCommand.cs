namespace StockMapSvelte.Application.UseCases.StockUseCases.Commands;

public class EditStockCommand
{
    public Guid Id { get; set; }
    public string TickerSymbol { get; set; } = string.Empty;
}