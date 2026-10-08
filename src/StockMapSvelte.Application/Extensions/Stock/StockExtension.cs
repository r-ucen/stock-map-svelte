using StockMapSvelte.Application.DTOs;

namespace StockMapSvelte.Application.Extensions.Stock;

public static class StockExtension
{
    extension(Domain.Entities.Stock s)
    {
        public TreemapNodeDto ToTreemapNodeDto()
        {
            return new TreemapNodeDto
            {
                TickerSymbol = s.TickerSymbol,
                Sector = s.StockProfile.Sector ?? "Unknown",
                FullName = s.StockProfile.FullName ?? string.Empty,
                MarketCap = s.StockProfile.MarketCap ?? 0,
                RegularMarketChangePercent = s.StockProfile.RegularMarketChangePercent,
                RegularMarketPrice = s.StockProfile.RegularMarketPrice,
                PreMarketChangePercent = s.StockProfile.PreMarketChangePercent,
                PreMarketPrice = s.StockProfile.PreMarketPrice,
                PostMarketChangePercent = s.StockProfile.PostMarketChangePercent,
                PostMarketPrice = s.StockProfile.PostMarketPrice,
                MarketState = s.StockProfile.MarketState,
                Currency = s.StockProfile.Currency,
                Volume = s.StockProfile.Volume,
                DividendDate = s.StockProfile.DividendDate,
                ExDividendDate = s.StockProfile.ExDividendDate,
                DividendYield = s.StockProfile.DividendYield,
                EarningsDate = s.StockProfile.EarningsDate,
                Beta = s.StockProfile.Beta,
                Pe = s.StockProfile.Pe,
                ForwardPe = s.StockProfile.ForwardPe,
                ShortRatio = s.StockProfile.ShortRatio,
                AnalystRecommendationMean = s.StockProfile.AnalystRecommendationMean,
                AnalystRecommendationKey = s.StockProfile.AnalystRecommendationKey,
                ProfitMargins = s.StockProfile.ProfitMargins,
                EarningsQuarterlyGrowth = s.StockProfile.EarningsQuarterlyGrowth,
                TrailingEps = s.StockProfile.TrailingEps,
                ForwardEps = s.StockProfile.ForwardEps,
                PegRatio = s.StockProfile.PegRatio,
                OneYearChange = s.StockProfile.OneYearChange,
                TargetHighPrice = s.StockProfile.TargetHighPrice,
                TargetLowPrice = s.StockProfile.TargetLowPrice,
                TargetMeanPrice = s.StockProfile.TargetMeanPrice,
                TargetMedianPrice = s.StockProfile.TargetMedianPrice,
                TotalDebt = s.StockProfile.TotalDebt,
                FreeCashflow = s.StockProfile.FreeCashflow,
                EarningsGrowth = s.StockProfile.EarningsGrowth,
                RevenueGrowth = s.StockProfile.RevenueGrowth
            };
        }
        
        public StockStockProfileDto ToStockStockProfileDto()
        {
            return new StockStockProfileDto
            {
                TickerSymbol = s.TickerSymbol,
                Date = s.StockProfile.Date,
                FullName = s.StockProfile.FullName,
                Sector = s.StockProfile.Sector,
                Currency = s.StockProfile.Currency,
                RegularMarketChangePercent = s.StockProfile.RegularMarketChangePercent,
                RegularMarketPrice = s.StockProfile.RegularMarketPrice,
                EarningsDate = s.StockProfile.EarningsDate,
                DividendDate = s.StockProfile.DividendDate,
                ExDividendDate = s.StockProfile.ExDividendDate,
                DividendYield = s.StockProfile.DividendYield,
                Beta = s.StockProfile.Beta,
                Pe = s.StockProfile.Pe,
                ForwardPe = s.StockProfile.ForwardPe,
                ShortRatio = s.StockProfile.ShortRatio,
                AnalystRecommendationMean = s.StockProfile.AnalystRecommendationMean,
                AnalystRecommendationKey = s.StockProfile.AnalystRecommendationKey,
                Volume = s.StockProfile.Volume,
                ProfitMargins = s.StockProfile.ProfitMargins,
                EarningsQuarterlyGrowth = s.StockProfile.EarningsQuarterlyGrowth,
                TrailingEps = s.StockProfile.TrailingEps,
                ForwardEps = s.StockProfile.ForwardEps,
                PegRatio = s.StockProfile.PegRatio,
                OneYearChange = s.StockProfile.OneYearChange,
                TargetHighPrice = s.StockProfile.TargetHighPrice,
                TargetLowPrice = s.StockProfile.TargetLowPrice,
                TargetMeanPrice = s.StockProfile.TargetMeanPrice,
                TargetMedianPrice = s.StockProfile.TargetMedianPrice,
                TotalDebt = s.StockProfile.TotalDebt,
                FreeCashflow = s.StockProfile.FreeCashflow,
                EarningsGrowth = s.StockProfile.EarningsGrowth,
                RevenueGrowth = s.StockProfile.RevenueGrowth
            };
        }
    }
}