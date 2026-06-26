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
        if (string.IsNullOrWhiteSpace(cmd.TickerSymbol))
        {
            throw new MissingTickerSymbolException("Ticker symbol field is required.");
        }
        
        var ticker = cmd.TickerSymbol.Trim().ToUpperInvariant();

        if (!await _stockClient.TickerExists(ticker))
        {
            throw new InvalidTickerSymbolException($"This ticker symbol: '{ticker}' is not valid.");
        }
        
        var exists = await _stockRepository.StockExistsAsync(ticker, cancellationToken);
        if (exists)
        {
            throw new TickerSymbolAlreadyExists($"Stock with ticker symbol '{cmd.TickerSymbol}' already exists.");
        }

        var entity = new Stock
        {
            TickerSymbol = ticker,
            Id = Guid.NewGuid()
        };
        
        var createStockResult = await _stockRepository.CreateStockAsync(entity);
        if (createStockResult <= 0)
        {
            throw new CreateStockFailException("Failed to create the stock.");
        }

        return new StockDto(
            entity.Id,
            entity.TickerSymbol
        );
    }
}