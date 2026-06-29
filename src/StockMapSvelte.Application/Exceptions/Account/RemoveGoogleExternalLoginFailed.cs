using StockMapSvelte.Domain.Exceptions;

namespace StockMapSvelte.Application.Exceptions.Account;

public class RemoveGoogleExternalLoginFailed : AppException
{
    public RemoveGoogleExternalLoginFailed(string message) : base(message, 500) { }
}