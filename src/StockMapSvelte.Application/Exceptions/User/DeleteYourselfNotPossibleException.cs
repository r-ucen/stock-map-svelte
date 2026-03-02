namespace StockMapSvelte.Application.Exceptions.User;

public class DeleteYourselfNotPossibleException : Exception
{
    public DeleteYourselfNotPossibleException(string message) : base(message)
    {
    }
}