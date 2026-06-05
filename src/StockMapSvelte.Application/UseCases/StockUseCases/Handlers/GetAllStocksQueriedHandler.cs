using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.DTOs.Common;


namespace StockMapSvelte.Application.UseCases.StockUseCases.Handlers;

public class GetAllStocksQueriedHandler
{
    private readonly IStockRepository _stockRepository;
    
    public GetAllStocksQueriedHandler(IStockRepository stockRepository)
    {
        _stockRepository = stockRepository;
    }
    
    public async Task<PagedResponse<StockDto>> Handle(QueryFilter filter, CancellationToken cancellationToken)
    {
        return await _stockRepository.GetAllStocksAsyncQueried(filter, cancellationToken);
    }
}