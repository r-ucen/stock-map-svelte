namespace StockMapSvelte.Application.Exceptions.Stock;

public class TickerSymbolAlreadyExists : Exception
{
    public string TickerSymbol { get; } = string.Empty;
    
    public TickerSymbolAlreadyExists(string message)
        : base(message)
    {
    }
    
    public TickerSymbolAlreadyExists(string message, string tickerSymbol)
        : base(message)
    {
        TickerSymbol = tickerSymbol;
    }
}