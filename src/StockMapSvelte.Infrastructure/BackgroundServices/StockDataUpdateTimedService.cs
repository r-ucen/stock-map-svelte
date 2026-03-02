using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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

        await DoWork();

        using PeriodicTimer timer = new(TimeSpan.FromSeconds(30));

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await DoWork();
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("StockDataUpdateTimedService is stopping.");
        }
    }

    private async Task DoWork()
    {
        try
        {
            int count = Interlocked.Increment(ref _executionCount);

            using var scope = _scopeFactory.CreateScope();
            var stockUpdateService = scope.ServiceProvider.GetRequiredService<Application.Abstractions.IStockUpdateService>();
            _logger.LogInformation("Starting stock update...");
            await stockUpdateService.UpdateAsync();
            _logger.LogInformation("StockDataUpdateTimedService finished. Run total of: {Count} times", count);
        } catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred executing StockDataUpdateTimedService.");
        }
    }
}