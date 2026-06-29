using StockMapSvelte.Domain.Exceptions;

namespace StockMapSvelte.Application.Exceptions.Stock;

public class StockDeletionFailedException : AppException
{
    public StockDeletionFailedException(string message) : base(message, 500) {}
}