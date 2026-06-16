using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.Exceptions.Stock;
using StockMapSvelte.Application.UseCases.StockUseCases.Commands;
using StockMapSvelte.Domain.Entities;

namespace StockMapSvelte.Application.UseCases.StockUseCases.Handlers;

public class CreateStockHandler
{
    private readonly IStockRepository _stockRepository;
    private readonly IStockProfileRepository _stockProfileRepository;
    private readonly IStockClient _stockClient;
    
    public CreateStockHandler(
        IStockRepository stockRepository,
        IStockProfileRepository stockProfileRepository,
        IStockClient stockClient)
    {
        _stockRepository = stockRepository;
        _stockProfileRepository = stockProfileRepository;
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
        
        var stockProfiles = await _stockClient.GetStockProfilesAsync(cancellationToken);

        var saveStockProfilesResult = await _stockProfileRepository.SaveStockProfilesAsync(stockProfiles, cancellationToken);
        if (saveStockProfilesResult <= 0)
        {
            throw new CreateStockFailException("Failed to update the additional stock data");
        }

        return new StockDto(
            entity.Id,
            entity.TickerSymbol
        );
    }
}