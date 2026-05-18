using StockMapSvelte.Application.DTOs;

namespace StockMapSvelte.Application.UseCases.AccountActionUseCases.Queries;

public class GetAccountInfoQuery
{
    public string? UserId { get; set; }
}