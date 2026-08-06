using StockMapSvelte.Domain.Exceptions;

namespace StockMapSvelte.Application.Exceptions.User;

public class UserNotBannedException : AppException
{
    public UserNotBannedException(string message) : base(message, 400) { }
}