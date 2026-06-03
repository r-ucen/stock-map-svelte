using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StockMapSvelte.Infrastructure.Services;

namespace StockMapSvelte.Infrastructure.BackgroundServices;

public class StockDataUpdateTimedService : BackgroundService
{
    private readonly ILogger<StockDataUpdateTimedService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private int _executionCount;

    public StockDataUpdateTimedService(
        ILogger<StockDataUpdateTimedService> logger,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("StockDataUpdateTimedService running");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var (shouldExecute, delayMinutes) = StockUpdateJitter.GetVariableDelayInMinutes();

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
        var stockUpdateService = scope.ServiceProvider.GetRequiredService<Application.Abstractions.IStockUpdateService>();
        _logger.LogInformation("Starting stock update...");
        await stockUpdateService.UpdateAsync(cancellationToken);
        _logger.LogInformation("StockDataUpdateTimedService finished. Run total of: {Count} times", count);
    }
}