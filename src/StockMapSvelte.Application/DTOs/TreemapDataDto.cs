namespace StockMapSvelte.Application.DTOs;

public class TreemapNodeDto
{
    public string TickerSymbol { get; set; } = string.Empty;
    public string Sector { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public double MarketCap { get; set; }

    public RectangleDto Rectangle { get; set; } = new RectangleDto();

    public double? RegularMarketChangePercent { get; set; }
    public decimal? RegularMarketPrice { get; set; }
    public double? PreMarketChangePercent { get; set; }
    public decimal? PreMarketPrice { get; set; }
    public double? PostMarketChangePercent { get; set; }
    public decimal? PostMarketPrice { get; set; }
    public string? MarketState { get; set; }
    public string? Currency { get; set; }
    public long? Volume { get; set; }
    public DateTimeOffset? DividendDate { get; set; }
    public DateTimeOffset? ExDividendDate { get; set; }
    public DateTimeOffset? EarningsDate { get; set; }
    public double? DividendYield { get; set; }
    public double? Beta { get; set; }
    public double? Pe { get; set; }
    public double? ForwardPe { get; set; }
    public double? ShortRatio { get; set; }
    public double? AnalystRecommendationMean { get; set; }
    public string? AnalystRecommendationKey { get; set; }
    
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

public class TreemapSectorDto
{
    public string SectorName { get; set; } = string.Empty;
    public double TotalMarketCap { get; set; }
    public List<TreemapNodeDto> Stocks { get; set; } = [];
    public RectangleDto Rectangle { get; set; } = new RectangleDto();
}

public class TreemapDataDto
{
    public List<TreemapSectorDto> Sectors { get; set; } = [];
    public double TotalMarketCap { get; set; }
}

public class RectangleDto
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
}