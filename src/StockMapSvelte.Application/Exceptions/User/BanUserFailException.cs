using StockMapSvelte.Domain.Exceptions;

namespace StockMapSvelte.Application.Exceptions.User;

public class BanUserFailException : AppException
{
    public BanUserFailException(string message) : base(message, 500) { }
}