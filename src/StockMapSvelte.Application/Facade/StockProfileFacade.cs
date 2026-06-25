using StockMapSvelte.Application.Abstractions.Facades;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.DTOs.Common;
using StockMapSvelte.Application.UseCases.StockProfileUseCases.Handlers;

namespace StockMapSvelte.Application.Facade;

public class StockProfileFacade : IStockProfileFacade
{
    private readonly GetAllStockProfilesHandler _getAllStockProfilesHandler;
    
    public StockProfileFacade(
        GetAllStockProfilesHandler getAllStockProfilesHandler)
    {
        _getAllStockProfilesHandler = getAllStockProfilesHandler;
    }
    
    public async Task<PagedResponse<StockStockProfileDto>> GetAllStockProfilesQueriedAsync(QueryFilter filter, CancellationToken cancellationToken)
    {
        return await _getAllStockProfilesHandler.Handle(filter, cancellationToken);
    }
}