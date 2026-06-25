using StockMapSvelte.Application.UseCases.StockUseCases.Queries;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.Exceptions.Stock;

namespace StockMapSvelte.Application.UseCases.StockUseCases.Handlers;

public class GetStockHandler
{
    private readonly IStockRepository _stockRepository;
    
    public GetStockHandler(IStockRepository stockRepository)
    {
        _stockRepository = stockRepository;
    }

    public async Task<StockDto> Handle(GetStockQuery query, CancellationToken cancellationToken)
    {
        return await _stockRepository.GetStockViewModelByIdAsync(query.StockId, cancellationToken) 
               ?? throw new StockNotFoundException($"Stock with id '{query.StockId}' was not found.", query.StockId.ToString());
    }
}