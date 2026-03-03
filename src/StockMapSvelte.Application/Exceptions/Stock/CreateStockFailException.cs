namespace StockMapSvelte.Application.Exceptions.Stock;

public class CreateStockFailException : AppException
{
    public CreateStockFailException(string message) : base(message, 500) { }
}