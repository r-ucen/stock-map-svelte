using StockMapSvelte.Domain.Exceptions;

namespace StockMapSvelte.Application.Exceptions.Portfolio;

public class PortfolioUnchangedException : AppException
{
    public PortfolioUnchangedException(string message) : base(message, 409) { }
}