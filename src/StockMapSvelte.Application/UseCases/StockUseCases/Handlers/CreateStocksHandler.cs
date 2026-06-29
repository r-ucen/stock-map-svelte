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
            try
            {
                if (!await _stockClient.TickerExists(tickerSymbol))
                {
                    failed.Add(new StockCreationFailure(tickerSymbol, "Not a valid ticker symbol"));
                    continue;
                }

                var exists = await _stockRepository.StockExistsAsync(tickerSymbol, cancellationToken);
                if (exists)
                {
                    failed.Add(new StockCreationFailure(tickerSymbol, "Already exists"));
                    continue;
                }

                var stock = Stock.Create(tickerSymbol);

                var createStockResult = await _stockRepository.CreateStockAsync(stock);
                if (createStockResult <= 0)
                {
                    failed.Add(new StockCreationFailure(tickerSymbol, "Unknown error"));
                    continue;
                }

                created.Add(stock.TickerSymbol);
            }
            catch (ArgumentException ex)
            {
                failed.Add(new StockCreationFailure(tickerSymbol, $"Malformed symbol format"));
                _logger.LogWarning(ex, "Malformed symbol format: {msg}",  ex.Message);
            }
        }
        
        return new CreateStocksResponse(created.ToArray(), failed.ToArray());
    }
}