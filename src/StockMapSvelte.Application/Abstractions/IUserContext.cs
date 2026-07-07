namespace StockMapSvelte.Application.Abstractions;

public interface IUserContext
{
    Task<string> GetCurrentUserIdAsync();
}