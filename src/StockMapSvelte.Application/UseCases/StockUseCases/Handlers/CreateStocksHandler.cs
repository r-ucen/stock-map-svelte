using Microsoft.Extensions.Logging;
using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Repositories;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.UseCases.StockUseCases.Commands;
using StockMapSvelte.Domain.Entities;

namespace StockMapSvelte.Application.UseCases.StockUseCases.Handlers;

public class CreateStocksHandler
{
    private readonly IStockRepository _stockRepository;
    private readonly IStockClient _stockClient;
    private readonly ILogger<CreateStocksHandler> _logger;
    
    public CreateStocksHandler(
        IStockRepository stockRepository,
        IStockClient stockClient,
        ILogger<CreateStocksHandler> logger)
    {
        _stockRepository = stockRepository;
        _stockClient = stockClient;
        _logger = logger;
    }

    public async Task<CreateStocksResponse> Handle(CreateStocksCommand cmd, CancellationToken cancellationToken)
    {
        var tickers = cmd.TickerSymbols?
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s.Trim().ToUpperInvariant())
            .Where(s => s.Length > 0 && char.IsLetterOrDigit(s[0]))
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
            try 
            {
                if (!await _stockClient.TickerExists(ticker))
                {
                    failed.Add(new StockCreationFailure(ticker, "Not a valid ticker symbol"));
                    continue;
                }
            }
            catch (ArgumentException ex)
            {
                failed.Add(new StockCreationFailure(ticker, $"Malformed symbol format"));
                _logger.LogWarning(ex, "Malformed symbol format: {msg}",  ex.Message);
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
        
        return new CreateStocksResponse(created.ToArray(), failed.ToArray());
    }
}