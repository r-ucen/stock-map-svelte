using StockMapSvelte.Application.Abstractions.Facades;
using StockMapSvelte.Application.DTOs;
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
    
    public async Task<IReadOnlyList<StockStockProfileDto>> GetAllStockProfilesAsync(CancellationToken cancellationToken)
    {
        return await _getAllStockProfilesHandler.Handle(cancellationToken);
    }
}