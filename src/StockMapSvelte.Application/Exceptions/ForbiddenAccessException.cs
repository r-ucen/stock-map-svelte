using StockMapSvelte.Domain.Exceptions;

namespace StockMapSvelte.Application.Exceptions;

public class ForbiddenAccessException : AppException
{
    public ForbiddenAccessException(string message = "Forbidden Access") : base(message, 403) { }
}