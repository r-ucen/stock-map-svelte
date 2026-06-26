using StockMapSvelte.Domain.Exceptions;

namespace StockMapSvelte.Application.Exceptions.User;

public class UnableToSetRoleException : AppException
{
    public UnableToSetRoleException(string message) : base(message, 500) { }
}