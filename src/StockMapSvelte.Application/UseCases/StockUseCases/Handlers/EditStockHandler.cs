using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Exceptions.Stock;
using StockMapSvelte.Application.UseCases.StockUseCases.Commands;

namespace StockMapSvelte.Application.UseCases.StockUseCases.Handlers;

public class EditStockHandler
{
    private readonly IStockClient _stockClient;

    private readonly IUnitOfWork _unitOfWork;
    
    public EditStockHandler(
        IStockClient stockClient,
        IUnitOfWork unitOfWork)
    {
        _stockClient = stockClient;
        _unitOfWork = unitOfWork;
    }
    
    public async Task Handle(EditStockCommand cmd, CancellationToken cancellationToken)
    {
        var stock = await _unitOfWork.Stocks.GetByIdAsync(cmd.Id, cancellationToken);
        if (stock is null) { throw new StockNotFoundException($"Stock with id '{cmd.Id}' was not found"); }
        
        var normalizedTicker = cmd.TickerSymbol.Trim().ToUpperInvariant();

        var tickerSymbolExists = await _unitOfWork.Stocks.ExistsAsync(normalizedTicker, cancellationToken);
        if (tickerSymbolExists) { throw new TickerSymbolAlreadyExists($"Stock with ticker symbol '{normalizedTicker}' already exists."); }

        var isValidTickerSymbol = await _stockClient.TickerSymbolExists(normalizedTicker);
        if (!isValidTickerSymbol) { throw new InvalidTickerSymbolException($"This ticker symbol: '{normalizedTicker}' is not valid."); }
        
        stock.Update(normalizedTicker);
        
        var editStockResult = await _unitOfWork.CommitAsync(cancellationToken);
        if (editStockResult <= 0) { throw new EditStockFailException("Failed to edit the stock."); }
    }
}