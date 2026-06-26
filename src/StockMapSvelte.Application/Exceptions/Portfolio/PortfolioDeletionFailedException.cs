using StockMapSvelte.Domain.Exceptions;

namespace StockMapSvelte.Application.Exceptions.Portfolio;

public class PortfolioDeletionFailedException : AppException
{
    public PortfolioDeletionFailedException(string message) : base(message, 500) { }
}