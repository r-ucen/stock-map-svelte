using StockMapSvelte.Application.Abstractions.Facades;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.UseCases.StockUseCases.Commands;
using StockMapSvelte.Application.UseCases.StockUseCases.Handlers;
using StockMapSvelte.Application.UseCases.StockUseCases.Queries;

namespace StockMapSvelte.Application.Facade;

public class StockFacade : IStockFacade
{
    private readonly GetAllStocksHandler _getAllStocksHandler;
    private readonly CreateStockHandler _createStockHandler;
    private readonly DeleteStockHandler _deleteStockHandler;
    private readonly EditStockHandler _editStockHandler;
    private readonly GetStockHandler _getStockHandler;
    private readonly GetPossibleToAddStocksHandler _getPossibleToAddStocksHandler;
    
    public StockFacade(
        GetAllStocksHandler getAllStocksHandler,
        CreateStockHandler createStockHandler,
        DeleteStockHandler deleteStockHandler,
        EditStockHandler editStockHandler,
        GetStockHandler getStockHandler,
        GetPossibleToAddStocksHandler getPossibleToAddStocksHandler)
    {
        _getAllStocksHandler = getAllStocksHandler;
        _createStockHandler = createStockHandler;
        _deleteStockHandler = deleteStockHandler;
        _editStockHandler = editStockHandler;
        _getStockHandler = getStockHandler;
        _getPossibleToAddStocksHandler = getPossibleToAddStocksHandler;
    }
    
    public async Task<IReadOnlyList<StockDto>> GetAllStocksAsync()
    {
        return await _getAllStocksHandler.Handle();
    }

    public async Task CreateStockAsync(string ticker)
    {
        var cmd = new CreateStockCommand
        {
            TickerSymbol = ticker
        };

        await _createStockHandler.Handle(cmd);
    }

    public async Task DeleteStockAsync(Guid stockId)
    {
        var cmd = new DeleteStockCommand
        {
            StockId = stockId
        };

        await _deleteStockHandler.Handle(cmd);
    }

    public async Task EditStockAsync(Guid stockId, string ticker)
    {
        var cmd = new EditStockCommand
        {
            Id = stockId,
            TickerSymbol = ticker
        };
        
        await _editStockHandler.Handle(cmd);
    }

    public async Task<StockDto> GetStockViewModelByIdAsync(Guid stockId)
    {
        var query = new GetStockQuery
        {
            StockId = stockId
        };
        
        return await _getStockHandler.Handle(query);
    }
    
    public async Task<IReadOnlyList<StockDto>> GetPossibleToAddStocksAsync(string filter, IList<string> stocksInPortfolio, CancellationToken cancellationToken)
    {
        var query = new GetPossibleToAddStocksQuery
        {
            Filter = filter,
            StocksInPortfolio = stocksInPortfolio,
            CancellationToken = cancellationToken
        };
        
        return await _getPossibleToAddStocksHandler.Handle(query);
    }
}