using StockMapSvelte.Domain.Exceptions;

namespace StockMapSvelte.Application.Exceptions.Portfolio;

public class PortfolioEditFailedException : AppException
{
    public PortfolioEditFailedException(string message) : base(message, 500) { }
}