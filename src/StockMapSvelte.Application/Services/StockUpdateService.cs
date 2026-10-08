using Microsoft.Extensions.Logging;
using StockMapSvelte.Application.Abstractions;

namespace StockMapSvelte.Application.Services;

public class StockUpdateService : IStockUpdateService
{
    private readonly IStockClient _stockClient;
    private readonly ILogger<StockUpdateService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStockProfileUpdateTracker _stockProfileUpdateTracker;

    public StockUpdateService(
        IStockClient stockClient,
        ILogger<StockUpdateService> logger,
        IUnitOfWork unitOfWork,
        IStockProfileUpdateTracker stockProfileUpdateTracker)
    {
        _stockClient = stockClient;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _stockProfileUpdateTracker = stockProfileUpdateTracker;
    }

    public async Task UpdateAsync(CancellationToken cancellationToken)
    {
        var stockProfiles = await _stockClient.GetStockProfilesAsync(cancellationToken);
        await _unitOfWork.StockProfiles.SaveAsync(stockProfiles, cancellationToken);
        
        var uninitializedStocks = await _unitOfWork.Stocks.GetUninitializedAsync(cancellationToken);

        if (uninitializedStocks.Count > 0)
        {
            var uninitializedStocksIds = uninitializedStocks.Select(s => s.Id).ToList();
            var numOfUpdated =
                await _unitOfWork.Stocks.MarkAsInitializedAsync(uninitializedStocksIds, cancellationToken);
            _logger.LogInformation("Marked {x} stocks as initialized. (first time fetching info for them)", numOfUpdated);
        }
        
        _stockProfileUpdateTracker.LastUpdated = DateTimeOffset.UtcNow;
    }
}