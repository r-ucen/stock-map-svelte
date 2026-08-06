using StockMapSvelte.Domain.Exceptions;

namespace StockMapSvelte.Application.Exceptions.User;

public class UnbanUserNotAllowed : AppException
{
    public UnbanUserNotAllowed(string message) : base(message, 403) { }
}