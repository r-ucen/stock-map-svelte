using StockMapSvelte.Application.UseCases.StockUseCases.Queries;
using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.DTOs;

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
        return await _stockRepository.GetStockViewModelByIdAsync(query.StockId, cancellationToken) ?? new StockDto(Guid.Empty, string.Empty);
    }
}