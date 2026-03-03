namespace StockMapSvelte.Application.Exceptions.Portfolio;

public class PortfolioCreationFailedException : AppException
{
    public PortfolioCreationFailedException(string message) : base(message, 500) {}
}