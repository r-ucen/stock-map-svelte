using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.Exceptions;

namespace StockMapSvelte.Infrastructure.Services.Trading212;

public class Trading212Client : ITrading212Client
{
    private readonly HttpClient _httpClient;
    private const string DemoUrl = "https://demo.trading212.com/api/v0";
    private const string LiveUrl = "https://live.trading212.com/api/v0";

    public Trading212Client(HttpClient httpClient)
    {
        _httpClient = httpClient;
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
    }

    public async Task<IEnumerable<Trading212PositionDto>?> GetPositionsAsync(string apiKey, string apiSecret, bool isDemo)
    {
        var baseUrl = isDemo
            ? DemoUrl
            : LiveUrl;
        
        var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/equity/positions");
        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{apiKey}:{apiSecret}"));
        request.Headers.Authorization = new AuthenticationHeaderValue(@"Basic", credentials);
        
        using var response = await _httpClient.SendAsync(request);
        
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) 
        { throw new UnauthorizedAccessException("Invalid Trading 212 API KEY ID or SECRET KEY."); }
        
        if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
        { throw new ForbiddenAccessException("Access to Trading 212 API is forbidden. Check key permissions."); }

        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadFromJsonAsync<IEnumerable<Trading212PositionDto>>();
    }
}