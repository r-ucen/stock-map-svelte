using StockMapSvelte.Domain.Exceptions;

namespace StockMapSvelte.Application.Exceptions.User;

public class BanUserNotAllowed : AppException
{
    public BanUserNotAllowed(string message) : base(message, 403) { }
}