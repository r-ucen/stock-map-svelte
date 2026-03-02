namespace StockMapSvelte.Application.Exceptions.User;

public class DeleteUserFailException : Exception
{
    public DeleteUserFailException(string message) : base(message)
    {
    }
}