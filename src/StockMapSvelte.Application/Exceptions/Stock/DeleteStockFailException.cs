using StockMapSvelte.Domain.Exceptions;

namespace StockMapSvelte.Application.Exceptions.Stock;

public class DeleteStockFailException : AppException
{
    public DeleteStockFailException(string message) : base(message, 500) { }
}