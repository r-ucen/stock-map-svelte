using System.Drawing;
using StockMapSvelte.Application.DTOs;
using StockMapSvelte.Application.Enums;

namespace StockMapSvelte.Application.Services.TreeMap.Coloring;

public class ColorScaleService
{
    public string GetCellColor(TreemapNodeDto stock, MapMetric mapMetric)
    {
        switch (mapMetric)
        {
            case MapMetric.RegularMarketChangePercent:
                if (!stock.RegularMarketChangePercent.HasValue)
                {
                    return ColorUtils.ToRgbaString(Color.FromArgb(0, 0, 0, 0));
                }

                Color negativeColorRegularMarketChangePercent = Color.FromArgb(255, 255, 0, 0);
                Color positiveColorRegularMarketChangePercent = Color.FromArgb(255, 0, 255, 0);

                var regularMarketChangePercent = stock.RegularMarketChangePercent.Value;

                const double loRegularMarketChangePercent = -5;
                const double hiRegularMarketChangePercent = 5;

                try
                {
                    return ColorUtils.ToRgbaString(ColorUtils.NegativeDecreasingAlphaTransparentIncreasingAlphaPositive(
                        regularMarketChangePercent,
                        loRegularMarketChangePercent,
                        hiRegularMarketChangePercent,
                        negativeColorRegularMarketChangePercent,
                        positiveColorRegularMarketChangePercent));
                }
                catch (ArgumentException)
                {
                    return ColorUtils.ToRgbaString(Color.FromArgb(0, 0, 0, 0));
                }

            case MapMetric.PreMarketChangePercent:
                if (!stock.PreMarketChangePercent.HasValue)
                {
                    return ColorUtils.ToRgbaString(Color.FromArgb(0, 0, 0, 0));
                }
                
                Color negativeColorPreMarketChangePercent = Color.FromArgb(255, 255, 0, 0);
                Color positiveColorPreMarketChangePercent = Color.FromArgb(255, 0, 255, 0);

                var preMarketChangePercent = stock.PreMarketChangePercent.Value;

                const double loPreMarketChangePercent = -5;
                const double hiPreMarketChangePercent = 5;
                
                try
                {
                    return ColorUtils.ToRgbaString(ColorUtils.NegativeDecreasingAlphaTransparentIncreasingAlphaPositive(
                        preMarketChangePercent,
                        loPreMarketChangePercent,
                        hiPreMarketChangePercent,
                        negativeColorPreMarketChangePercent,
                        positiveColorPreMarketChangePercent));
                }
                catch (ArgumentException)
                {
                    return ColorUtils.ToRgbaString(Color.FromArgb(0, 0, 0, 0));
                }
                
            case MapMetric.PostMarketChangePercent:
                if (!stock.PostMarketChangePercent.HasValue)
                {
                    return ColorUtils.ToRgbaString(Color.FromArgb(0, 0, 0, 0));
                }
                
                Color negativeColorPostMarketChangePercent = Color.FromArgb(255, 255, 0, 0);
                Color positiveColorPostMarketChangePercent = Color.FromArgb(255, 0, 255, 0);

                var postMarketChangePercent = stock.PostMarketChangePercent.Value;

                const double loPostMarketChangePercent = -5;
                const double hiPostMarketChangePercent = 5;
                
                try
                {
                    return ColorUtils.ToRgbaString(ColorUtils.NegativeDecreasingAlphaTransparentIncreasingAlphaPositive(
                        postMarketChangePercent,
                        loPostMarketChangePercent,
                        hiPostMarketChangePercent,
                        negativeColorPostMarketChangePercent,
                        positiveColorPostMarketChangePercent));
                }
                catch (ArgumentException)
                {
                    return ColorUtils.ToRgbaString(Color.FromArgb(0, 0, 0, 0));
                }
                
            case MapMetric.MarketState:
                if (string.IsNullOrEmpty(stock.MarketState))
                {
                    return ColorUtils.ToRgbaString(Color.FromArgb(0, 0, 0, 0));
                }
                
                var marketState = stock.MarketState.Trim().ToUpper();

                return marketState switch
                {
                    "REGULAR" => ColorUtils.ToRgbaString(Color.FromArgb(255, 41, 144, 59)),
                    "PRE" => ColorUtils.ToRgbaString(Color.FromArgb(255, 41, 98, 255)),
                    "POST" => ColorUtils.ToRgbaString(Color.FromArgb(255, 255, 152, 0)),
                    "CLOSED" => ColorUtils.ToRgbaString(Color.FromArgb(255, 75, 75, 75)),
                    _ => ColorUtils.ToRgbaString(Color.FromArgb(0, 0, 0, 0))
                };

            case MapMetric.Volume:
                if (!stock.Volume.HasValue)
                {
                    return ColorUtils.ToRgbaString(Color.FromArgb(0, 0, 0, 0));
                }

                Color colorVolume = Color.FromArgb(255, 0, 178, 255);
                var volume = stock.Volume.Value;
                
                const long hiVolume = 50_000_000;

                try
                {
                    return ColorUtils.ToRgbaString(ColorUtils.PositiveIncreasingAlpha(volume, hiVolume, colorVolume));
                } catch (ArgumentException)
                {
                    return ColorUtils.ToRgbaString(Color.FromArgb(0, 0, 0, 0));
                }
                
            case MapMetric.DividendDate:
                if (!stock.DividendDate.HasValue)
                {
                    return ColorUtils.ToRgbaString(Color.FromArgb(0, 0, 0, 0));
                }

                return DateColorScale.DateCaseColor(stock.DividendDate.Value);
            
            case MapMetric.ExDividendDate:
                if (!stock.ExDividendDate.HasValue)
                {
                    return ColorUtils.ToRgbaString(Color.FromArgb(0, 0, 0, 0));
                }
                
                return  DateColorScale.DateCaseColor(stock.ExDividendDate.Value);
            
            case MapMetric.DividendYield:
                if (!stock.DividendYield.HasValue)
                {
                    return ColorUtils.ToRgbaString(Color.FromArgb(0, 0, 0, 0));
                }
                
                var dividendYieldPercentage = stock.DividendYield.Value * 100;
                var hiDividendYieldPercentage = 8.0;

                try
                {
                    return ColorUtils.ToRgbaString(ColorUtils.PositiveIncreasingAlpha(
                        dividendYieldPercentage,
                        hiDividendYieldPercentage,
                        Color.FromArgb(255, 0, 255, 0)));
                }
                catch (Exception)
                {
                    return ColorUtils.ToRgbaString(Color.FromArgb(0, 0, 0, 0));
                }
            
            case MapMetric.EarningsDate:
                if (!stock.EarningsDate.HasValue)
                {
                    return ColorUtils.ToRgbaString(Color.FromArgb(0, 0, 0, 0));
                }
                
                return DateColorScale.DateCaseColor(stock.EarningsDate.Value);        

            case MapMetric.Beta:
                if (!stock.Beta.HasValue)
                {
                    return ColorUtils.ToRgbaString(Color.FromArgb(0, 0, 0, 0));
                }

                const double bottomBeta = -1.0;
                const double middleBeta = 1.0;
                const double topBeta = 2.5;
                var colorBottomBeta = Color.FromArgb(255, 255, 81, 0);
                var colorMiddleBeta = Color.FromArgb(255, 255, 0, 174);
                var colorTopBeta = Color.FromArgb(255, 0, 255, 169);
                
                return ColorUtils.ToRgbaString(ColorUtils.ThreeWayScale(
                    stock.Beta.Value,
                    bottomBeta,
                    middleBeta,
                    topBeta,
                    colorBottomBeta,
                    colorMiddleBeta,
                    colorTopBeta
                    ));
            
            case MapMetric.Pe:
                if (!stock.Pe.HasValue)
                {
                    return ColorUtils.ToRgbaString(Color.FromArgb(0, 0, 0, 0));
                }
                
                var pe = stock.Pe.Value;
                var loPe = 0.0;
                var hiPe = 40.0;
                var colorPe = Color.FromArgb(255, 0, 255, 0);

                try
                {
                    return ColorUtils.ToRgbaString(ColorUtils.PositiveDecreasingAlpha(pe, loPe, hiPe, colorPe));
                } catch (Exception)
                {
                    return ColorUtils.ToRgbaString(Color.FromArgb(0, 0, 0, 0));
                }
                
            case MapMetric.ForwardPe:
                if (!stock.ForwardPe.HasValue)        
                {
                    return ColorUtils.ToRgbaString(Color.FromArgb(0, 0, 0, 0));
                }
                
                var forwardPe = stock.ForwardPe.Value;
                var loForwardPe = 0.0;
                var hiForwardPe = 30.0;
                var colorForwardPe = Color.FromArgb(255, 0, 255, 0);

                try
                {
                    return ColorUtils.ToRgbaString(ColorUtils.PositiveDecreasingAlpha(forwardPe, loForwardPe, hiForwardPe, colorForwardPe));
                } catch (Exception)
                {
                    return ColorUtils.ToRgbaString(Color.FromArgb(0, 0, 0, 0));
                }
                
            case MapMetric.ShortRatio:
                if (!stock.ShortRatio.HasValue)        
                {
                    return ColorUtils.ToRgbaString(Color.FromArgb(0, 0, 0, 0));
                }

                var shortRatio = stock.ShortRatio.Value;
                var loShortRatio = 0.0;
                var hiShortRatio = 12.0;
                var colorShortRatio = Color.FromArgb(255, 0, 255, 0);
                
                return ColorUtils.ToRgbaString(ColorUtils.PositiveDecreasingAlpha(shortRatio, loShortRatio, hiShortRatio, colorShortRatio));

            default:
                return ColorUtils.ToRgbaString(Color.FromArgb(0, 0, 0, 0));
        }
    }
}