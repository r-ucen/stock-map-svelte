namespace StockMapSvelte.Application.DTOs;

public record StockStockProfileDto
{
    public string? TickerSymbol { get; set; }
    public DateTimeOffset? Date { get; set; }
    public string? FullName { get; set; }
    public string? Sector { get; set; }
    public string? Currency { get; set; }
    public double? RegularMarketChangePercent { get; set; }
    public decimal? RegularMarketPrice { get; set; }
    public DateTimeOffset? EarningsDate { get; set; }
    public DateTimeOffset? DividendDate { get; set; }
    public DateTimeOffset? ExDividendDate { get; set; }
    public double? DividendYield { get; set; }
    public double? Beta { get; set; }
    public double? Pe { get; set; }
    public double? ForwardPe { get; set; }
    public double? ShortRatio { get; set; }
    public double? AnalystRecommendationMean { get; set; }
    public string? AnalystRecommendationKey { get; set; }
    public long? Volume { get; set; }
    public double? ProfitMargins { get; set; }
    public double? EarningsQuarterlyGrowth { get; set; }
    public double? TrailingEps { get; set; }
    public double? ForwardEps { get; set; }
    public double? PegRatio { get; set; }
    public double? OneYearChange { get; set; }
    
    public double? TargetHighPrice { get; set; }
    public double? TargetLowPrice { get; set; }
    public double? TargetMeanPrice { get; set; }
    public double? TargetMedianPrice { get; set; }
    
    public double? TotalDebt { get; set; }
    public double? FreeCashflow { get; set; }
    public double? EarningsGrowth { get; set; }
    public double? RevenueGrowth { get; set; }
}