using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StockMapSvelte.Application.Abstractions;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Domain.Entities;
using StockMapSvelte.Infrastructure.Database;
using YahooQuotesApi;

namespace StockMapSvelte.Infrastructure.Services.Yahoo;

public class YahooStockClient(
    ApplicationDbContext dbContext,
    YahooQuotes yahooQuotes,
    IHttpClientFactory httpClientFactory,
    ILogger<YahooStockClient> logger)
    : IStockClient
{
    public async Task<string> GetYahooTickerSymbol(string isin)
    { 
        var httpClient = httpClientFactory.CreateClient("YahooSearchClient");
        var url = $"https://query2.finance.yahoo.com/v1/finance/search?q={isin}&quotesCount=1&newsCount=0";
        
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        using var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        
        var responseContent = await response.Content.ReadFromJsonAsync<YahooSearchResponse>();
        if (responseContent?.Quotes is not { Length: > 0 }) return string.Empty;
        
        var ticker = responseContent.Quotes[0].Symbol;
        
        return !string.IsNullOrWhiteSpace(ticker) ? ticker : string.Empty;
    }
    
    public async Task<bool> TickerExists(string ticker)
    {
        var snapshots = await yahooQuotes.GetSnapshotAsync([ticker]);

        foreach (var snapshot in snapshots)
        {
            if (snapshot.Value != null)
            {
                var modules = await yahooQuotes.GetModulesAsync(snapshot.Key, ["assetProfile", "summaryDetail", "defaultKeyStatistics", "financialData"]);

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

    public async Task<IReadOnlyList<StockProfile>> GetStockProfilesAsync(CancellationToken cancellationToken)
    {
        var stocksByTicker = await dbContext.Stocks
            .AsNoTracking()
            .ToDictionaryAsync(s => s.TickerSymbol, s => s, cancellationToken);

        if (stocksByTicker.Count == 0) { return []; }

        var snapshots = await yahooQuotes.GetSnapshotAsync(stocksByTicker.Keys, cancellationToken);

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
                    Volume = null,
                    ProfitMargins = null,
                    EarningsQuarterlyGrowth = null,
                    TrailingEps = null,
                    ForwardEps = null,
                    PegRatio = null,
                    OneYearChange = null,
                    TargetHighPrice = null,
                    TargetLowPrice = null,
                    TargetMeanPrice = null,
                    TargetMedianPrice = null,
                    TotalDebt = null,
                    FreeCashflow = null,
                    EarningsGrowth = null,
                    RevenueGrowth = null
                };

                var modules = await yahooQuotes.GetModulesAsync(snapshot.Key, ["assetProfile", "summaryDetail", "defaultKeyStatistics", "financialData"], cancellationToken);

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
                        logger.LogTrace("No Ex Dividend Date data found for {name}.", snapshot.Value);
                    }

                    // DIVIDEND YIELD

                    var dividendYield = GetDoubleFromProperty(summaryDetail.Value, "dividendYield");
                    if (dividendYield.HasValue)
                    {
                        profile.DividendYield = dividendYield;
                    }
                    else
                    {
                        logger.LogTrace("No Dividend Yield data found for {name}.", snapshot.Value);
                    }

                    // BETA

                    var beta = GetDoubleFromProperty(summaryDetail.Value, "beta");
                    if (beta.HasValue)
                    {
                        profile.Beta = beta;
                    }
                    else
                    {
                        logger.LogTrace("No Beta data found for {name}.", snapshot.Value);
                    }

                    // P/E
                    var trailingPe = snapshot.Value.TrailingPE;
                    profile.Pe = trailingPe;

                    // FORWARD P/E
                    var forwardPe = snapshot.Value.ForwardPE;
                    profile.ForwardPe = forwardPe;

                    // SHORT RATIO
                    var shortRatio = GetDoubleFromProperty(defaultKeyStatistics.Value, "shortRatio");
                    if (shortRatio.HasValue)
                    {
                        profile.ShortRatio = shortRatio;
                    }
                    else
                    {
                        logger.LogTrace("No ShortRatio data found for {name}.", snapshot.Value);
                    }

                    // VOLUME
                    var volume = snapshot.Value.RegularMarketVolume;
                    profile.Volume = volume;
                    
                    
                    var profitMargins = GetDoubleFromProperty(defaultKeyStatistics.Value, "profitMargins");
                    if (shortRatio.HasValue)
                    {
                        profile.ProfitMargins = profitMargins;
                    }
                    else
                    {
                        logger.LogTrace("No profitMargins data found for {name}.", snapshot.Value);
                    }
                
                    var earningsQuarterlyGrowth = GetDoubleFromProperty(defaultKeyStatistics.Value, "earningsQuarterlyGrowth");
                    if (earningsQuarterlyGrowth.HasValue)                    {
                        profile.EarningsQuarterlyGrowth = earningsQuarterlyGrowth;
                    }
                    else                    {
                        logger.LogTrace("No earningsQuarterlyGrowth data found for {name}.", snapshot.Value);
                    }
                    
                    var trailingEps = GetDoubleFromProperty(defaultKeyStatistics.Value, "trailingEps");
                    if (trailingEps.HasValue)
                    {
                        profile.TrailingEps = trailingEps;
                    }
                    else
                    {
                        logger.LogTrace("No trailingEps data found for {name}.", snapshot.Value);
                    }
                    
                    var forwardEps = GetDoubleFromProperty(defaultKeyStatistics.Value, "forwardEps");
                    if (forwardEps.HasValue)
                    {
                        profile.ForwardEps = forwardEps;
                    }
                    else {
                        logger.LogTrace("No forwardEps data found for {name}.", snapshot.Value);
                    }
                    
                    var pegRatio = GetDoubleFromProperty(defaultKeyStatistics.Value, "pegRatio");
                    if (pegRatio.HasValue)
                    {
                        profile.PegRatio = pegRatio;
                    }
                    else {
                        logger.LogTrace("No pegRatio data found for {name}.", snapshot.Value);
                    }
                    
                    var oneYearChange = GetDoubleFromProperty(defaultKeyStatistics.Value, "52WeekChange");
                    if (oneYearChange.HasValue)
                    {
                        profile.OneYearChange = oneYearChange;
                    }
                    else {
                        logger.LogTrace("No 52WeekChange data found for {name}.", snapshot.Value);
                    }
                    
                    var targetHighPrice = GetDoubleFromProperty(financialData.Value, "targetHighPrice");
                    if (targetHighPrice.HasValue)
                    {
                        profile.TargetHighPrice = targetHighPrice;
                    }
                    else
                    {
                        logger.LogTrace("No targetHighPrice data found for {name}.", snapshot.Value);
                    }
                    
                    var targetLowPrice = GetDoubleFromProperty(financialData.Value, "targetLowPrice");
                    if (targetLowPrice.HasValue)
                    {
                        profile.TargetLowPrice = targetLowPrice;
                    }
                    else                    {
                        logger.LogTrace("No targetLowPrice data found for {name}.", snapshot.Value);
                    }
                    
                    var targetMeanPrice = GetDoubleFromProperty(financialData.Value, "targetMeanPrice");
                    if (targetMeanPrice.HasValue)
                    {
                        profile.TargetMeanPrice = targetMeanPrice;
                    }
                    else                    {
                        logger.LogTrace("No targetMeanPrice data found for {name}.", snapshot.Value);
                    }
                    
                    var targetMedianPrice = GetDoubleFromProperty(financialData.Value, "targetMedianPrice");
                    if (targetMedianPrice.HasValue)
                    {
                        profile.TargetMedianPrice = targetMedianPrice;
                    }
                    else                    {
                        logger.LogTrace("No targetMedianPrice data found for {name}.", snapshot.Value);
                    }
                    
                    var analystRecommendationMean = GetDoubleFromProperty(financialData.Value, "recommendationMean");
                    if (analystRecommendationMean.HasValue)
                    {
                        profile.AnalystRecommendationMean = analystRecommendationMean;
                    }
                    else                    {
                        logger.LogTrace("No recommendationMean data found for {name}.", snapshot.Value);
                    }

                    try
                    {
                        var recommendationKey = financialData.Value
                            .GetProperty("recommendationKey")
                            .GetString();
                        if (recommendationKey == "none") { recommendationKey = null; }
                        profile.AnalystRecommendationKey = recommendationKey;
                    }
                    catch (Exception)
                    {
                        logger.LogTrace("No recommendationKey data found for {name}.", snapshot.Value);
                    }
                    
                    var totalDebt = GetDoubleFromProperty(financialData.Value, "totalDebt");
                    if (totalDebt.HasValue)
                    {
                        profile.TotalDebt = totalDebt;
                    }
                    else
                    {
                        logger.LogTrace("No totalDebt data found for {name}.", snapshot.Value);
                    }
                    
                    var freeCashflow = GetDoubleFromProperty(financialData.Value, "freeCashflow");
                    if (freeCashflow.HasValue)
                    {
                        profile.FreeCashflow = freeCashflow;
                    }
                    else                    {
                        logger.LogTrace("No freeCashflow data found for {name}.", snapshot.Value);
                    }
                    
                    var earningsGrowth = GetDoubleFromProperty(financialData.Value, "earningsGrowth");
                    if (earningsGrowth.HasValue)
                    {
                        profile.EarningsGrowth = earningsGrowth;
                    }
                    else                    {
                        logger.LogTrace("No earningsGrowth data found for {name}.", snapshot.Value);
                    }
                    
                    var revenueGrowth = GetDoubleFromProperty(financialData.Value, "revenueGrowth");
                    if (revenueGrowth.HasValue)
                    {
                        profile.RevenueGrowth = revenueGrowth;
                    }
                    else
                    {
                        logger.LogTrace("No revenueGrowth data found for {name}.", snapshot.Value);
                    }
                }

                result.Add(profile);
            }
            else
            {
                logger.LogTrace(" No data found for {name}.", snapshot.Value);
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