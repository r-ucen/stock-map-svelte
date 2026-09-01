using StockMapSvelte.Domain.Exceptions;

namespace StockMapSvelte.Application.Exceptions.UserSetting;

public class GetDefaultPortfolioIdFailedException : AppException
{
    public GetDefaultPortfolioIdFailedException() : base("Failed to retrieve default portfolio id.", 500) { }
}