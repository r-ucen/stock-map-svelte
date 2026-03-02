namespace StockMapSvelte.Application.Exceptions.Portfolio;

public class PortfolioNotFoundException : Exception
{
    public PortfolioNotFoundException(string message) : base(message)
    {
    }
}