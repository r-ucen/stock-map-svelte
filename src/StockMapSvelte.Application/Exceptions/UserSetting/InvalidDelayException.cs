namespace StockMapSvelte.Application.Exceptions.UserSetting;

public class InvalidDelayException : AppException
{
    public InvalidDelayException(int delay)
        : base($"Invalid delay value: {delay}. Delay must be greater than 500 ms and less than 60000 ms.", 400) { }
}