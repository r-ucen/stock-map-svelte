using Microsoft.Extensions.Logging;
using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.UseCases.StockUseCases.Commands;
using StockMapSvelte.Domain.Entities;

namespace StockMapSvelte.Application.UseCases.StockUseCases.Handlers;

public class CreateStocksHandler
{
    private readonly IStockClient _stockClient;
    private readonly ILogger<CreateStocksHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    
    public CreateStocksHandler(
        IStockClient stockClient,
        ILogger<CreateStocksHandler> logger,
        IUnitOfWork unitOfWork)
    {
        _stockClient = stockClient;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateStocksResponse> Handle(CreateStocksCommand cmd, CancellationToken cancellationToken)
    {
        var tickers = cmd.TickerSymbols?
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s.Trim().ToUpperInvariant())
            .Where(s => s.Length > 0 && char.IsLetterOrDigit(s[0]))
            .ToArray() ?? [];

        if (tickers.Length == 0) { return new CreateStocksResponse([], []); }
        
        var created = new List<string>();
        var failed = new List<StockCreationFailure>();
        
        foreach (var tickerSymbol in tickers)
        {
            try
            {
                if (!await _stockClient.TickerSymbolExists(tickerSymbol))
                {
                    failed.Add(new StockCreationFailure(tickerSymbol, "Not a valid ticker symbol"));
                    continue;
                }

                var exists = await _unitOfWork.Stocks.ExistsAsync(tickerSymbol, cancellationToken);
                if (exists)
                {
                    failed.Add(new StockCreationFailure(tickerSymbol, "Already exists"));
                    continue;
                }

                var stock = Stock.Create(tickerSymbol);
                
                await _unitOfWork.Stocks.AddAsync(stock, cancellationToken);
                created.Add(stock.TickerSymbol);
            }
            catch (ArgumentException ex)
            {
                failed.Add(new StockCreationFailure(tickerSymbol, $"Malformed symbol format"));
                _logger.LogWarning(ex, "Malformed symbol format: {msg}",  ex.Message);
            }
        }
        
        await _unitOfWork.CommitAsync(cancellationToken);
        
        return new CreateStocksResponse(created.ToArray(), failed.ToArray());
    }
}