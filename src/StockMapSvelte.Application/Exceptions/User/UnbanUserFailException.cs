using StockMapSvelte.Domain.Exceptions;

namespace StockMapSvelte.Application.Exceptions.User;

public class UnbanUserFailException : AppException
{
    public UnbanUserFailException(string message) : base(message, 500) { }
}