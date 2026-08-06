using StockMapSvelte.Domain.Exceptions;

namespace StockMapSvelte.Application.Exceptions.User;

public class BanYourselfNotPossibleException : AppException
{
    public BanYourselfNotPossibleException(string message) : base(message, 400) { }
}