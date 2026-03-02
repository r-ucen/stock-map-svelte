namespace StockMapSvelte.Application.Exceptions.Stock;

public class MissingTickerSymbolException : Exception
{
    public string TickerSymbol { get; } = string.Empty;

    public MissingTickerSymbolException(string message)
        : base(message)
    {
    }

    public MissingTickerSymbolException(string message, string tickerSymbol)
        : base(message)
    {
        TickerSymbol = tickerSymbol;
    }
}