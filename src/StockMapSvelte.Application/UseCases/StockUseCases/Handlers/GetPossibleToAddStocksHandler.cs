using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.UseCases.StockUseCases.Queries;

namespace StockMapSvelte.Application.UseCases.StockUseCases.Handlers;

public class GetPossibleToAddStocksHandler
{
    private readonly IStockRepository _stockRepository;
    private readonly IUserContext _userContext;
    
    public GetPossibleToAddStocksHandler(IStockRepository stockRepository, IUserContext userContext)
    {
        _stockRepository = stockRepository;
        _userContext = userContext;
    }
    
    public async Task<IReadOnlyList<StockDto>> Handle(GetPossibleToAddStocksQuery query)
    {
        if (!await _userContext.IsUserAuthenticatedAsync())
        {
            throw new UnauthorizedAccessException($"User {await _userContext.GetCurrentUserIdAsync()} is not authenticated to get possible to add stocks.");
        }
        
        return await _stockRepository.GetPossibleToAddStocksAsync(query.Filter, query.StocksInPortfolio, query.CancellationToken);
    }
}