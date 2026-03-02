using System.Globalization;
using System.Text.Json;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Domain.Entities;
using StockMapSvelte.Infrastructure.Database;
using YahooQuotesApi;

namespace StockMapSvelte.Infrastructure.Repositories;

public class YahooStockClient : IStockClient
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly YahooQuotes _yahooQuotes;
    public YahooStockClient(IDbContextFactory<ApplicationDbContext> contextFactory, YahooQuotes yahooQuotes)
    {
        _contextFactory = contextFactory;
        _yahooQuotes = yahooQuotes;
    }

    public async Task<bool> TickerExists(string ticker)
    {
        var snapshots = await _yahooQuotes.GetSnapshotAsync([ticker]);

        foreach (var snapshot in snapshots)
        {
            if (snapshot.Value != null)
            {
                var modules = await _yahooQuotes.GetModulesAsync(snapshot.Key, ["assetProfile", "summaryDetail", "defaultKeyStatistics", "financialData"]);

                if (!modules.HasError && modules.Value.Length > 0)
                {
                    var fullName = snapshot.Value.LongName;
                    var regularMarketPrice = snapshot.Value.RegularMarketPrice;
                    if (!string.IsNullOrWhiteSpace(fullName) && !string.IsNullOrWhiteSpace(regularMarketPrice.ToString("N")))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public async Task<IReadOnlyList<StockProfile>> GetStockProfilesAsync()
    {
        await using var _dbContext = await _contextFactory.CreateDbContextAsync();

        var stocksByTicker = await _dbContext.Stocks
            .AsNoTracking()
            .ToDictionaryAsync(s => s.TickerSymbol!, s => s);

        if (stocksByTicker.Count == 0)
        {
            return [];
        }

        var snapshots = await _yahooQuotes.GetSnapshotAsync(stocksByTicker.Keys);

        var result = new List<StockProfile>();
        var now = DateTimeOffset.UtcNow;

        foreach (var snapshot in snapshots)
        {
            if (snapshot.Value != null)
            {
                var parentStock = stocksByTicker[snapshot.Key];

                StockProfile profile = new()
                {
                    Id = NewId.NextGuid(),
                    StockId = parentStock.Id,
                    Date = now,
                    FullName = null,
                    MarketCap = null,
                    Sector = null,
                    Currency = null,
                    MarketState = null,
                    RegularMarketChangePercent = null,
                    RegularMarketPrice = null,
                    PreMarketChangePercent = null,
                    PreMarketPrice = null,
                    PostMarketChangePercent = null,
                    PostMarketPrice = null,
                    EarningsDate = null,
                    DividendDate = null,
                    ExDividendDate = null,
                    DividendYield = null,
                    Beta = null,
                    Pe = null,
                    ForwardPe = null,
                    ShortRatio = null,
                    AnalystRecommendationMean = null,
                    AnalystRecommendationKey = null,
                    Volume = null
                };

                var modules = await _yahooQuotes.GetModulesAsync(snapshot.Key, ["assetProfile", "summaryDetail", "defaultKeyStatistics", "financialData"]);

                if (!modules.HasError && modules.Value.Length > 0)
                {
                    var assetProfileProp = modules.Value[0];

                    profile.Sector = assetProfileProp.Value
                        .GetProperty("sector")
                        .GetString();

                    var assetProfile = modules.Value[0];
                    var summaryDetail = modules.Value[1];
                    var defaultKeyStatistics = modules.Value[2];
                    var financialData = modules.Value[3];

                    // FULL NAME
                    var fullName = snapshot.Value.LongName;
                    profile.FullName = fullName;
                    
                    var marketCap = snapshot.Value.MarketCap;
                    profile.MarketCap = marketCap;

                    // SECTOR
                    var sector = assetProfile.Value
                        .GetProperty("sector")
                        .GetString();
                    profile.Sector = sector;

                    // CURRENCY
                    var currency = financialData.Value
                        .GetProperty("financialCurrency")
                        .GetString();
                    profile.Currency = currency;
                    
                    // MARKET STATE
                    var marketState = snapshot.Value.MarketState;
                    profile.MarketState = marketState;

                    // REGULAR MARKET CHANGE PERCENT
                    var regularMarketChangePercent = snapshot.Value.RegularMarketChangePercent;
                    profile.RegularMarketChangePercent = regularMarketChangePercent;

                    // REGULAR MARKET PRICE
                    var regularMarketPrice = snapshot.Value.RegularMarketPrice;
                    profile.RegularMarketPrice = regularMarketPrice;
                    
                    // PRE MARKET CHANGE PERCENT
                    var preMarketChangePercent = snapshot.Value.PreMarketChangePercent;
                    profile.PreMarketChangePercent = preMarketChangePercent;
                    
                    // PRE MARKET PRICE
                    var preMarketPrice = snapshot.Value.PreMarketPrice;
                    profile.PreMarketPrice = preMarketPrice;
                    
                    // POST MARKET CHANGE PERCENT
                    var postMarketChangePercent = snapshot.Value.PostMarketChangePercent;
                    profile.PostMarketChangePercent = postMarketChangePercent;
                    
                    // POST MARKET PRICE
                    var postMarketPrice = snapshot.Value.PostMarketPrice;
                    profile.PostMarketPrice = postMarketPrice;

                    // EARNINGS DATE
                    var earningsDateS = snapshot.Value.EarningsTimestamp.ToString();
                    if (TryParseToDateTimeOffset(earningsDateS, out DateTimeOffset earningsDate))
                    {
                        profile.EarningsDate = earningsDate;
                    }

                    // DIVIDEND DATE
                    var dividendDateS = snapshot.Value.DividendDate.ToString();
                    if (TryParseToDateTimeOffset(dividendDateS, out DateTimeOffset dividendDate))
                    {
                        profile.DividendDate = dividendDate;
                    }

                    // EX DIVIDEND DATE
                    try
                    {
                        var exDividendDateS = summaryDetail.Value
                            .GetProperty("exDividendDate")
                            .GetProperty("raw").ToString();

                        var exDividendDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(exDividendDateS));
                        profile.ExDividendDate = exDividendDate;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("No Ex Dividend Date data found.");
                    }

                    // DIVIDEND YIELD

                    var dividendYield = GetDoubleFromProperty(summaryDetail.Value, "dividendYield");
                    if (dividendYield.HasValue)
                    {
                        profile.DividendYield = dividendYield;
                    }
                    else
                    {
                        Console.WriteLine("No Dividend Yield data found.");
                    }

                    // BETA

                    var beta = GetDoubleFromProperty(summaryDetail.Value, "beta");
                    if (beta.HasValue)
                    {
                        profile.Beta = beta;
                    }
                    else
                    {
                        Console.WriteLine("No Beta data found.");
                    }

                    // P/E
                    var trailingPe = snapshot.Value.TrailingPE;
                    profile.Pe = trailingPe;

                    // FORWARD P/E
                    var forwardPe = snapshot.Value.ForwardPE;
                    profile.ForwardPe = forwardPe;

                    // SHORT RATIO
                    var shortRatio = GetDoubleFromProperty(defaultKeyStatistics.Value, "shortRatio");
                    if (beta.HasValue)
                    {
                        profile.ShortRatio = shortRatio;
                    }
                    else
                    {
                        Console.WriteLine("No ShortRatio data found.");
                    }

                    // ANALYSTS RECOMMENDATION MEAN
                    var recommendationMean = GetDoubleFromProperty(financialData.Value, "recommendationMean");
                    if (recommendationMean.HasValue)
                    {
                        profile.AnalystRecommendationMean = recommendationMean;
                    }
                    else
                    {
                        Console.WriteLine("No Recommendation Mean data found.");
                    }

                    // ANALYSTS RECOMMENDATION KEY
                    var recommendationKey = financialData.Value
                        .GetProperty("recommendationKey")
                        .GetString();
                    profile.AnalystRecommendationKey = recommendationKey;

                    // VOLUME
                    var volume = snapshot.Value.RegularMarketVolume;
                    profile.Volume = volume;
                }

                result.Add(profile);
            }
            else
            {
                Console.Write(" No data found.");
            }
        }
        return result;
    }


    static bool TryParseToDateTimeOffset(string? isoOrNull, out DateTimeOffset dto)
    {
        dto = default;
        if (string.IsNullOrWhiteSpace(isoOrNull)) return false;
        return DateTimeOffset.TryParse(isoOrNull,
            null,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
            out dto);
    }

    static double? GetDoubleFromProperty(JsonElement parent, string propName)
    {
        if (!parent.TryGetProperty(propName, out var prop)) return null;

        if (prop.ValueKind == JsonValueKind.Number)
        {
            if (prop.TryGetDouble(out var d)) return d;
            return null;
        }

        if (prop.ValueKind == JsonValueKind.Object && prop.TryGetProperty("raw", out var raw))
        {
            if (raw.ValueKind == JsonValueKind.Number && raw.TryGetDouble(out var rd)) return rd;
            if (raw.ValueKind == JsonValueKind.String && double.TryParse(raw.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var rs)) return rs;
            return null;
        }

        if (prop.ValueKind == JsonValueKind.String)
        {
            var s = prop.GetString();
            if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var sd)) return sd;
        }

        return null;
    }
}