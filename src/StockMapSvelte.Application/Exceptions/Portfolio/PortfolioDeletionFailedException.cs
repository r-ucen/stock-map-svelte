namespace StockMapSvelte.Application.Exceptions.Portfolio;

public class PortfolioDeletionFailedException : Exception
{
    public PortfolioDeletionFailedException(string message)
        : base(message)
    {
    }
}