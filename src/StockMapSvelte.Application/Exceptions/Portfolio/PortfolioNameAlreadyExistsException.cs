namespace StockMapSvelte.Application.Exceptions.Portfolio;

public class PortfolioNameAlreadyExistsException : AppException
{
    public PortfolioNameAlreadyExistsException(string portfolioName) : base($"Portfolio with name '{portfolioName}' already exists.", 409) { }
}