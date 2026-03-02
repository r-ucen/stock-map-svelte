namespace StockMapSvelte.Application.Exceptions.Portfolio;

public class PortfolioNameMissingException : Exception
{
    public PortfolioNameMissingException()
        : base("Portfolio name is missing.")
    {
    }
}