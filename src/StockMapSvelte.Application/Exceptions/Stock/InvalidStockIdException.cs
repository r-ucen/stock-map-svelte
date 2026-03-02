namespace StockMapSvelte.Application.Exceptions.Stock;

public class InvalidStockIdException : Exception
{
    public Guid StockId { get; }
    public InvalidStockIdException(string message)
        : base(message)
    {
    }
    public InvalidStockIdException(string message, Guid stockId)
        : base(message)
    {
        StockId = stockId;
    }
}