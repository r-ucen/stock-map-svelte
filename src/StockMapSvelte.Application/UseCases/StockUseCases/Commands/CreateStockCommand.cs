namespace StockMapSvelte.Application.UseCases.StockUseCases.Commands;

public class CreateStockCommand
{
    public string TickerSymbol { get; set; } = string.Empty;
}