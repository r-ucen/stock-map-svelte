using StockMapSvelte.Application.Abstractions.Facades;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.DTOs.Common;
using StockMapSvelte.Application.UseCases.StockUseCases.Commands;
using StockMapSvelte.Application.UseCases.StockUseCases.Handlers;
using StockMapSvelte.Application.UseCases.StockUseCases.Queries;

namespace StockMapSvelte.Application.Facade;

public class StockFacade : IStockFacade
{
    private readonly GetAllStocksHandler _getAllStocksHandler;
    private readonly GetAllStocksQueriedHandler _getAllStocksQueriedHandler;
    private readonly CreateStockHandler _createStockHandler;
    private readonly DeleteStockHandler _deleteStockHandler;
    private readonly EditStockHandler _editStockHandler;
    private readonly GetStockHandler _getStockHandler;
    private readonly GetPossibleToAddStocksHandler _getPossibleToAddStocksHandler;
    
    public StockFacade(
        GetAllStocksHandler getAllStocksHandler,
        GetAllStocksQueriedHandler getAllStocksQueriedHandler,
        CreateStockHandler createStockHandler,
        DeleteStockHandler deleteStockHandler,
        EditStockHandler editStockHandler,
        GetStockHandler getStockHandler,
        GetPossibleToAddStocksHandler getPossibleToAddStocksHandler)
    {
        _getAllStocksHandler = getAllStocksHandler;
        _getAllStocksQueriedHandler = getAllStocksQueriedHandler;
        _createStockHandler = createStockHandler;
        _deleteStockHandler = deleteStockHandler;
        _editStockHandler = editStockHandler;
        _getStockHandler = getStockHandler;
        _getPossibleToAddStocksHandler = getPossibleToAddStocksHandler;
    }
    
    public async Task<IReadOnlyList<StockDto>> GetAllStocksAsync(CancellationToken cancellationToken)
    {
        return await _getAllStocksHandler.Handle(cancellationToken);
    }
    
    public async Task<PagedResponse<StockDto>> GetAllStocksQueriedAsync(QueryFilter filter, CancellationToken cancellationToken)
    {
        return await _getAllStocksQueriedHandler.Handle(filter, cancellationToken);
    }

    public async Task<StockDto> CreateStockAsync(CreateStockCommand cmd, CancellationToken cancellationToken)
    {
        return await _createStockHandler.Handle(cmd, cancellationToken);
    }

    public async Task DeleteStockAsync(DeleteStockCommand cmd, CancellationToken cancellationToken)
    {
        await _deleteStockHandler.Handle(cmd, cancellationToken);
    }

    public async Task EditStockAsync(EditStockCommand cmd, CancellationToken cancellationToken)
    {
        await _editStockHandler.Handle(cmd, cancellationToken);
    }

    public async Task<StockDto> GetStockViewModelByIdAsync(Guid stockId, CancellationToken cancellationToken)
    {
        var query = new GetStockQuery(stockId);
        
        return await _getStockHandler.Handle(query, cancellationToken);
    }
    
    public async Task<IReadOnlyList<StockDto>> GetPossibleToAddStocksAsync(GetPossibleToAddStocksQuery query, CancellationToken cancellationToken)
    {
        return await _getPossibleToAddStocksHandler.Handle(query, cancellationToken);
    }
}