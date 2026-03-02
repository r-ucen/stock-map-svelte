namespace StockMapSvelte.Application.Exceptions.Stock;

public class CreateStockFailException : Exception
{
    public CreateStockFailException(string message)
        : base(message)
    {
    }
}