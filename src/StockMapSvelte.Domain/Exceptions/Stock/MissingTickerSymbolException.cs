namespace StockMapSvelte.Domain.Exceptions.Stock;

public class MissingTickerSymbolException : AppException
{
    public MissingTickerSymbolException(string message) : base(message, 400) { }
}