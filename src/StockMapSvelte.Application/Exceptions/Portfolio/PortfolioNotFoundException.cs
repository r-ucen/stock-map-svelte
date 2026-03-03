namespace StockMapSvelte.Application.Exceptions.Portfolio;

public class PortfolioNotFoundException : AppException
{
    public PortfolioNotFoundException(string message) : base(message, 404) { }
}