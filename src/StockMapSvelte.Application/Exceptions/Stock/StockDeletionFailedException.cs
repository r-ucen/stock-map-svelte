namespace StockMapSvelte.Application.Exceptions.Stock;

public class StockDeletionFailedException : Exception
{
    public StockDeletionFailedException(string message) : base(message){}
}