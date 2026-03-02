namespace StockMapSvelte.Application.Exceptions.Stock;

public class StockNotFoundException : Exception
{
    public string TickerSymbol { get; } = string.Empty;
    
    public StockNotFoundException(string message)
        : base(message)
    {
    }
    
    public StockNotFoundException(string message, string tickerSymbol)
        : base(message)
    {
        TickerSymbol = tickerSymbol;
    }
}