namespace StockMapSvelte.Application.Exceptions.Stock;

public class InvalidTickerSymbolException : AppException
{
    public InvalidTickerSymbolException(string message) : base(message, 400) { }
}