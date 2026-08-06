using StockMapSvelte.Domain.Exceptions;

namespace StockMapSvelte.Application.Exceptions.User;

public class UnbanYourselfNotPossibleException : AppException
{
    public UnbanYourselfNotPossibleException(string message) : base(message, 403) { }
}