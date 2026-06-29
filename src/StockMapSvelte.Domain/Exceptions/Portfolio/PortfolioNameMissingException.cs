namespace StockMapSvelte.Domain.Exceptions.Portfolio;

public class PortfolioNameMissingException : AppException
{
    public PortfolioNameMissingException() : base("Portfolio name is missing.", 400) { }
}