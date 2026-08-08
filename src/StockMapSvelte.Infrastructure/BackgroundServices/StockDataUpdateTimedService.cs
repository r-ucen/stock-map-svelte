using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Infrastructure.Repositories.Cached.CacheManagement;
using StockMapSvelte.Infrastructure.Services;

namespace StockMapSvelte.Infrastructure.BackgroundServices;

public class StockDataUpdateTimedService : BackgroundService
{
    private readonly ILogger<StockDataUpdateTimedService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private int _executionCount;
    private readonly ITreeMapUpdateNotifier _treeMapUpdateNotifier;
    private readonly HybridCache _cache;
    private bool _isFirstRun = true;

    public StockDataUpdateTimedService(
        ILogger<StockDataUpdateTimedService> logger,
        IServiceScopeFactory scopeFactory,
        ITreeMapUpdateNotifier treeMapUpdateNotifier,
        HybridCache cache)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _treeMapUpdateNotifier = treeMapUpdateNotifier;
        _cache = cache;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("StockDataUpdateTimedService running");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var (shouldExecute, delayMinutes) = StockUpdateJitter.GetVariableDelayInMinutes(DateTimeOffset.UtcNow, ref _isFirstRun);

                if (shouldExecute)
                {
                    await DoWork(stoppingToken);
                }
                
                _logger.LogInformation("Waiting {Minutes} minutes until next run", delayMinutes);
                await Task.Delay(TimeSpan.FromMinutes(delayMinutes), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("StockDataUpdateTimedService is stopping.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred executing StockDataUpdateTimedService.");
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }

    private async Task DoWork(CancellationToken cancellationToken)
    {
        var count = Interlocked.Increment(ref _executionCount);
            
        using var scope = _scopeFactory.CreateScope();
        var stockUpdateService = scope.ServiceProvider.GetRequiredService<IStockUpdateService>();
        _logger.LogInformation("Starting stock update...");
        await stockUpdateService.UpdateAsync(cancellationToken);
        
        await _cache.RemoveByTagAsync(CacheTags.Portfolio.TreemapData, cancellationToken);
        _treeMapUpdateNotifier.Publish();
        
        _logger.LogInformation("StockDataUpdateTimedService finished. Run total of: {Count} times", count);
    }
}