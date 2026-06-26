using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.Exceptions.Stock;
using StockMapSvelte.Application.UseCases.StockUseCases.Commands;

namespace StockMapSvelte.Application.UseCases.StockUseCases.Handlers;

public class EditStockHandler
{
    private readonly IStockRepository _stockRepository;
    private readonly IStockClient _stockClient;
    
    public EditStockHandler(
        IStockRepository stockRepository,
        IStockClient stockClient)
    {
        _stockRepository = stockRepository;
        _stockClient = stockClient;
    }
    
    public async Task Handle(EditStockCommand cmd, CancellationToken cancellationToken)
    {
        if (!await _stockRepository.StockExistsAsync(cmd.Id, cancellationToken))
        {
            throw new StockNotFoundException($"Stock with id: '{cmd.Id}' was not found.", cmd.Id.ToString());
        }
        
        var normalizedTicker = cmd.TickerSymbol.Trim().ToUpperInvariant();
        
        if (await _stockRepository.StockExistsAsync(cmd.TickerSymbol, cancellationToken))
        {
            throw new TickerSymbolAlreadyExists($"Stock with ticker symbol '{cmd.TickerSymbol}' already exists.");
        }
        
        if (!await _stockClient.TickerExists(cmd.TickerSymbol))
        {
            throw new InvalidTickerSymbolException($"This ticker symbol: '{cmd.TickerSymbol}' is not valid.");
        }
        
        var editStockResult = await _stockRepository.EditStockAsync(cmd.Id, normalizedTicker, cancellationToken);
        if (editStockResult <= 0)
        {
            throw new Exception("Failed to edit the stock.");
        }
    }
}