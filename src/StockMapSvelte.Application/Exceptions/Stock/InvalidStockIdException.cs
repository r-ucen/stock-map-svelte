namespace StockMapSvelte.Application.Exceptions.Stock;

public class InvalidStockIdException : AppException
{
    public Guid StockId { get; }
    
    public InvalidStockIdException(string message, Guid stockId) : base(message, 400) { StockId = stockId; }
}