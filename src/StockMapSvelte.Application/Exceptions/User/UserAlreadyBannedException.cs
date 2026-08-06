using StockMapSvelte.Domain.Exceptions;

namespace StockMapSvelte.Application.Exceptions.User;

public class UserAlreadyBannedException : AppException
{
    public UserAlreadyBannedException(string message) : base(message, 400) { }
}