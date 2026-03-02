namespace StockMapSvelte.Application.Exceptions.Stock;

public class InvalidTickerSymbolException : Exception
{
    public string TickerSymbol { get; } = string.Empty;

    public InvalidTickerSymbolException(string message)
        : base(message)
    {
    }

    public InvalidTickerSymbolException(string message, string tickerSymbol)
        : base(message)
    {
        TickerSymbol = tickerSymbol;
    }
}