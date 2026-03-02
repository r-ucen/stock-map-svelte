namespace StockMapSvelte.Application.Exceptions.Portfolio;

public class PortfolioNameAlreadyExistsException : Exception
{
    public PortfolioNameAlreadyExistsException(string portfolioName)
        : base($"Portfolio with name '{portfolioName}' already exists.")
    {
    }
}