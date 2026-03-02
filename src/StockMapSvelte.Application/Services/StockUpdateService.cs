using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.Abstractions.Repositories;

namespace StockMapSvelte.Application.Services;

public class StockUpdateService : IStockUpdateService
{
    private readonly IStockClient _stockClient;
    private readonly IStockProfileRepository _stockProfileRepository;

    public StockUpdateService(
        IStockClient stockClient,
        IStockProfileRepository stockProfileRepository)
    {
        _stockClient = stockClient;
        _stockProfileRepository = stockProfileRepository;
    }

    public async Task UpdateAsync()
    {
        var stockProfiles = await _stockClient.GetStockProfilesAsync();
        await _stockProfileRepository.SaveStockProfilesAsync(stockProfiles);
    }
}