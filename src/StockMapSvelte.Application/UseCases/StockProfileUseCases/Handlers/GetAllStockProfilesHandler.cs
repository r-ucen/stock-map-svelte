using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.DTOs.Common;

namespace StockMapSvelte.Application.UseCases.StockProfileUseCases.Handlers;

public class GetAllStockProfilesHandler
{
    private readonly IStockProfileRepository _stockProfileRepository;
    
    public GetAllStockProfilesHandler(
        IStockProfileRepository stockProfileRepository)
    {
        _stockProfileRepository = stockProfileRepository;
    }
    
    public async Task<PagedResponse<StockStockProfileDto>> Handle(QueryFilter filter, CancellationToken cancellationToken)
    {
        return await _stockProfileRepository.GetAllStockProfilesAsync(filter, cancellationToken);
    }
        
}