namespace StockMapSvelte.Application.Exceptions.Stock;

public class TickerSymbolAlreadyExists : AppException
{
    public TickerSymbolAlreadyExists(string message) : base(message, 409) { }
}