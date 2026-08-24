using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.Exceptions.Stock;
using StockMapSvelte.Application.UseCases.StockUseCases.Commands;
using StockMapSvelte.Domain.Entities;

namespace StockMapSvelte.Application.UseCases.StockUseCases.Handlers;

public class CreateStockHandler
{
    private readonly IStockClient _stockClient;
    private readonly IUnitOfWork _unitOfWork;
    
    public CreateStockHandler(
        IStockClient stockClient,
        IUnitOfWork unitOfWork)
    {
        _stockClient = stockClient;
        _unitOfWork = unitOfWork;
    }

    public async Task<StockDto> Handle(CreateStockCommand cmd, CancellationToken cancellationToken)
    {
        var stock = Stock.Create(cmd.TickerSymbol);

        if (!await _stockClient.TickerSymbolExists(stock.TickerSymbol))
        {
            throw new InvalidTickerSymbolException($"This ticker symbol: '{stock.TickerSymbol}' is not valid.");
        }
        
        var exists = await _unitOfWork.Stocks.ExistsAsync(stock.TickerSymbol, cancellationToken);
        if (exists) { throw new TickerSymbolAlreadyExists($"Stock with ticker symbol '{stock.TickerSymbol}' already exists."); }
        
        
        await _unitOfWork.Stocks.AddAsync(stock, cancellationToken);
        
        var result = await _unitOfWork.CommitAsync(cancellationToken);
        if (result <= 0) { throw new CreateStockFailException("Failed to create the stock."); }

        return new StockDto(stock.Id, stock.TickerSymbol);
    }
}