using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.DTOs;


namespace StockMapSvelte.Application.UseCases.StockUseCases.Handlers;

public class GetAllStocksHandler
{
    private readonly IStockRepository _stockRepository;
    private readonly IUserContext _userContext;
    
    public GetAllStocksHandler(IStockRepository stockRepository, IUserContext userContext)
    {
        _stockRepository = stockRepository;
        _userContext = userContext;
    }
    
    public async Task<IReadOnlyList<StockDto>> Handle()
    {
        return await _stockRepository.GetAllStocksAsync() ?? new List<StockDto>();
    }
}