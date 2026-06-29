using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.Exceptions.Stock;
using StockMapSvelte.Application.UseCases.StockUseCases.Commands;
using StockMapSvelte.Domain.Entities;
using StockMapSvelte.Domain.Exceptions.Stock;

namespace StockMapSvelte.Application.UseCases.StockUseCases.Handlers;

public class CreateStockHandler
{
    private readonly IStockRepository _stockRepository;
    private readonly IStockClient _stockClient;
    
    public CreateStockHandler(
        IStockRepository stockRepository,
        IStockClient stockClient)
    {
        _stockRepository = stockRepository;
        _stockClient = stockClient;
    }

    public async Task<StockDto> Handle(CreateStockCommand cmd, CancellationToken cancellationToken)
    {
        var stock = Stock.Create(cmd.TickerSymbol);

        if (!await _stockClient.TickerExists(stock.TickerSymbol))
        {
            throw new InvalidTickerSymbolException($"This ticker symbol: '{stock.TickerSymbol}' is not valid.");
        }
        
        var exists = await _stockRepository.StockExistsAsync(stock.TickerSymbol, cancellationToken);
        if (exists)
        {
            throw new TickerSymbolAlreadyExists($"Stock with ticker symbol '{stock.TickerSymbol}' already exists.");
        }
        
        var createStockResult = await _stockRepository.CreateStockAsync(stock);
        if (createStockResult <= 0)
        {
            throw new CreateStockFailException("Failed to create the stock.");
        }

        return new StockDto(stock.Id, stock.TickerSymbol);
    }
}