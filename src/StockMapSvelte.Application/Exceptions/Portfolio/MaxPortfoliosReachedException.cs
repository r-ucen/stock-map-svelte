using StockMapSvelte.Domain.Exceptions;

namespace StockMapSvelte.Application.Exceptions.Portfolio;

public class MaxPortfoliosReachedException : AppException
{
    public MaxPortfoliosReachedException(int maxPortfolios) : base($"You cannot have more than {maxPortfolios} portfolios.", 409) { }
}