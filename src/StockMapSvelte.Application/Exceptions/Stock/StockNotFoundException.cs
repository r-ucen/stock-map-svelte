namespace StockMapSvelte.Application.Exceptions.Stock;

public class StockNotFoundException : AppException
{
    public string TickerSymbol { get; } = string.Empty;
    
    public StockNotFoundException(string message)
        : base(message, 404)
    {
    }
    
    public StockNotFoundException(string message, string tickerSymbol)
        : base(message, 404)
    {
        TickerSymbol = tickerSymbol;
    }
}