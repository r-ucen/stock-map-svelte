using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.UseCases.StockUseCases.Queries;

namespace StockMapSvelte.Application.UseCases.StockUseCases.Handlers;

public class GetPossibleToAddStocksHandler
{
    private readonly IStockRepository _stockRepository;
    
    public GetPossibleToAddStocksHandler(IStockRepository stockRepository)
    {
        _stockRepository = stockRepository;
    }
    
    public async Task<IReadOnlyList<StockDto>> Handle(GetPossibleToAddStocksQuery query, CancellationToken cancellationToken)
    {
        return await _stockRepository.GetPossibleToAddStocksAsync(query.Filter.ToUpper(), query.StocksInPortfolio, cancellationToken);
    }
}