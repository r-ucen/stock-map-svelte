using StockMapSvelte.Domain.Exceptions;

namespace StockMapSvelte.Application.Exceptions.UserSetting;

public class FailedToSetDefaultPortfolioException : AppException
{
    public FailedToSetDefaultPortfolioException(string message) : base(message, 500) {}
}