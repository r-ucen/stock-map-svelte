using StockMapSvelte.Domain.Exceptions;

namespace StockMapSvelte.Application.Exceptions.Stock;

public class EditStockFailException : AppException
{
    public EditStockFailException(string message) : base(message, 500) { }
}