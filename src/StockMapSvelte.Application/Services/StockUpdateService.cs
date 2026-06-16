using Microsoft.Extensions.Logging;
using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Repositories;

namespace StockMapSvelte.Application.Services;

public class StockUpdateService : IStockUpdateService
{
    private readonly IStockClient _stockClient;
    private readonly IStockProfileRepository _stockProfileRepository;
    private readonly IStockRepository _stockRepository;
    private readonly ILogger<StockUpdateService> _logger;

    public StockUpdateService(
        IStockClient stockClient,
        IStockProfileRepository stockProfileRepository,
        IStockRepository stockRepository,
        ILogger<StockUpdateService> logger)
    {
        _stockClient = stockClient;
        _stockProfileRepository = stockProfileRepository;
        _stockRepository = stockRepository;
        _logger = logger;
    }

    public async Task UpdateAsync(CancellationToken cancellationToken)
    {
        var stockProfiles = await _stockClient.GetStockProfilesAsync(cancellationToken);
        await _stockProfileRepository.SaveStockProfilesAsync(stockProfiles, cancellationToken);
        
        var uninitializedStocks = await _stockRepository.GetUninitializedStocksAsync(cancellationToken);

        if (uninitializedStocks.Count > 0)
        {
            var uninitializedStocksIds = uninitializedStocks.Select(s => s.Id).ToList();
            var numOfUpdated =
                await _stockRepository.MarkStocksAsInitializedAsync(uninitializedStocksIds, cancellationToken);
            _logger.LogInformation("Marked {x} stocks as initialized. (first time fetching info for them)", numOfUpdated);
        }
        
    }
}