using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.UseCases.StockUseCases.Commands;
using StockMapSvelte.Domain.Entities;

namespace StockMapSvelte.Application.UseCases.StockUseCases.Handlers;

public class CreateStocksHandler
{
    private readonly IStockRepository _stockRepository;
    private readonly IStockProfileRepository _stockProfileRepository;
    private readonly IStockClient _stockClient;
    
    public CreateStocksHandler(
        IStockRepository stockRepository,
        IStockProfileRepository stockProfileRepository,
        IStockClient stockClient)
    {
        _stockRepository = stockRepository;
        _stockProfileRepository = stockProfileRepository;
        _stockClient = stockClient;
    }

    public async Task<CreateStocksResponse> Handle(CreateStocksCommand cmd, CancellationToken cancellationToken)
    {
        var tickers = cmd.TickerSymbols?
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s.Trim().ToUpperInvariant())
            .ToArray() ?? Array.Empty<string>();

        if (tickers.Length == 0)
        {
            return new CreateStocksResponse([], []);
        }
        
        var created = new List<string>();
        var failed = new List<StockCreationFailure>();
        
        foreach (var tickerSymbol in tickers)
        {
            if (string.IsNullOrWhiteSpace(tickerSymbol))
            {
                continue;
            }
            
            var ticker = tickerSymbol.Trim().ToUpperInvariant();
            
            if (!await _stockClient.TickerExists(ticker))
            {
                failed.Add(new StockCreationFailure(ticker, "Not a valid ticker symbol"));
                continue;
            }
            
            var exists = await _stockRepository.StockExistsAsync(ticker, cancellationToken);
            if (exists)
            {
                failed.Add(new StockCreationFailure(ticker, "Already exists"));
                continue;
            }
            
            var entity = new Stock
            {
                TickerSymbol = ticker,
                Id = Guid.NewGuid()
            };
        
            var createStockResult = await _stockRepository.CreateStockAsync(entity);
            if (createStockResult <= 0)
            {
                failed.Add(new StockCreationFailure(ticker, "Unknown error"));
                continue;
            }
            
            created.Add(ticker);
        }

        if (created.Count > 0)
        {
            var stockProfiles = await _stockClient.GetStockProfilesAsync(cancellationToken);
            await _stockProfileRepository.SaveStockProfilesAsync(stockProfiles, cancellationToken);
        }
        
        return new CreateStocksResponse(created.ToArray(), failed.ToArray());
    }
}