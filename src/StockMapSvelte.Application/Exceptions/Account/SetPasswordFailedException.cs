using StockMapSvelte.Domain.Exceptions;

namespace StockMapSvelte.Application.Exceptions.Account;

public class SetPasswordFailedException : AppException
{
    public SetPasswordFailedException() : base("Failed to set password.", 500)
    { }
}