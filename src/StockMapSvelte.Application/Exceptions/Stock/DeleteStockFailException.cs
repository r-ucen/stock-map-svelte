namespace StockMapSvelte.Application.Exceptions.Stock;

public class DeleteStockFailException : Exception
{
    public DeleteStockFailException(string message)
        : base(message)
    {
    }
}