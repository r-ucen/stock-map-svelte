using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.Enums;

namespace StockMapSvelte.Application.Services.TreeMap.Formatting;

public class CellDescriptionFormatter
{
    public string CurrencyNameToSign(string currencyName)
    {
        switch (currencyName)
        {
            case "USD":
                return "$";
            default:
                return " ";
        }
    }

    public string GetCellDescription(TreemapNodeDto stock, MapMetric mapMetric)
    {
        switch (mapMetric)
        {
            case MapMetric.RegularMarketChangePercent:
                if (!stock.RegularMarketChangePercent.HasValue)
                {
                    return "\n-";
                }

                var regularMarketChangePercent = stock.RegularMarketChangePercent.Value;

                var regularMarketPrice = stock.RegularMarketPrice.HasValue
                    ? stock.RegularMarketPrice.Value.ToString("0.##")
                    : "N/A";

                return "\n" + regularMarketChangePercent.ToString("0.##") + "%" + "\n" + $"Price: {CurrencyNameToSign(stock.Currency ?? "")}" + regularMarketPrice;

            case MapMetric.PreMarketChangePercent:
                if (!stock.PreMarketChangePercent.HasValue)
                {
                    return "\n-";
                }

                var preMarketChangePercent = stock.PreMarketChangePercent.Value;

                if (stock.PreMarketPrice == null || stock.PreMarketPrice == 0)
                {
                    return "\n-";
                }
                
                var preMarketPriceString = stock.PreMarketPrice.Value.ToString("0.##");
                var preMarketChangePercentString = preMarketChangePercent.ToString("0.##");

                return "\n" + preMarketChangePercentString  + "%" + "\n" + $"Price: {CurrencyNameToSign(stock.Currency ?? "")}" + preMarketPriceString;
            
            case MapMetric.PostMarketChangePercent:
                if (!stock.PostMarketChangePercent.HasValue)
                {
                    return "\n-";
                }

                var postMarketChangePercent = stock.PostMarketChangePercent.Value;

                if (stock.PostMarketPrice == null || stock.PostMarketPrice == 0)
                {
                    return "\n-";
                }
                var postMarketPriceString = stock.PostMarketPrice.Value.ToString("0.##");
                var postMarketChangePercentString = postMarketChangePercent.ToString("0.##");

                return "\n" + postMarketChangePercentString  + "%" + "\n" + $"Price: {CurrencyNameToSign(stock.Currency ?? "")}" + postMarketPriceString;
            
            case MapMetric.MarketState:
                if (string.IsNullOrEmpty(stock.MarketState))
                {
                    return "\n-";
                }
                
                var marketState = stock.MarketState.Trim().ToUpper();

                return marketState switch
                {
                    "REGULAR" => "\nOPEN",
                    "PRE" => "\nPRE-MARKET",
                    "POST" => "\nPOST-MARKET",
                    "CLOSED" => "\nCLOSED",
                    _ => "\n-"
                };

            case MapMetric.Volume:
                if (!stock.Volume.HasValue)
                {
                    return "\n-";
                }

                var volume = stock.Volume.Value;
                return "\n" + volume.ToString("N0");
            
            case MapMetric.DividendDate:
                if (!stock.DividendDate.HasValue)
                {
                    return "\n-";
                }

                return DateDescriptionFormatter.DateCaseDescription(stock.DividendDate.Value, MapMetric.DividendDate);
            
            case MapMetric.ExDividendDate:
                if (!stock.ExDividendDate.HasValue)
                {
                    return "\n-";
                }

                return DateDescriptionFormatter.DateCaseDescription(stock.ExDividendDate.Value, MapMetric.ExDividendDate);
            
            case MapMetric.DividendYield:
                if (!stock.DividendYield.HasValue)
                {
                    return "\n-";
                }

                return "\n" + stock.DividendYield.Value.ToString("P");
            
            case MapMetric.EarningsDate:
                if (!stock.EarningsDate.HasValue)
                {
                    return "\n-";
                }

                return DateDescriptionFormatter.DateCaseDescription(stock.EarningsDate.Value, MapMetric.EarningsDate);
                
            case MapMetric.Beta:
                if (!stock.Beta.HasValue)
                {
                    return "\n-";
                }
                
                return "\n" + stock.Beta.Value.ToString("0.##");
            
            case MapMetric.Pe:
                if (!stock.Pe.HasValue)
                {
                    return "\n-";
                }
                
                return "\n" + stock.Pe.Value.ToString("0.##");
            
            case MapMetric.ForwardPe:
                if (!stock.ForwardPe.HasValue)
                {
                    return "\n-";
                }
                
                return "\n" + stock.ForwardPe.Value.ToString("0.##");
            
            case MapMetric.ShortRatio:
                if (!stock.ShortRatio.HasValue)
                {
                    return "\n-";
                }
                
                return "\n" + stock.ShortRatio.Value.ToString("0.##");
            
            default:
                return "\n-";
        }
    }
}