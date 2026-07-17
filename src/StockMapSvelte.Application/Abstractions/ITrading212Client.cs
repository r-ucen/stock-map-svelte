using StockMapSvelte.Application.DTOs;

namespace StockMapSvelte.Application.Abstractions;

public interface ITrading212Client
{
    public Task<IEnumerable<Trading212PositionDto>?> GetPositionsAsync(string apiKey, string apiSecret, bool isDemo);
}