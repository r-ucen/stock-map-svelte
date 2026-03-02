namespace StockMapSvelte.Application.Exceptions.Portfolio;

public class PortfolioCreationFailedException : Exception
{
    public PortfolioCreationFailedException(string message)
        : base(message)
    {
    }
}