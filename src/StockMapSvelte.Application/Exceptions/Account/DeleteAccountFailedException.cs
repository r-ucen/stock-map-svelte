using StockMapSvelte.Domain.Exceptions;

namespace StockMapSvelte.Application.Exceptions.Account;

public class DeleteAccountFailedException : AppException
{
    public DeleteAccountFailedException(string message) : base(message, 500) { }
}