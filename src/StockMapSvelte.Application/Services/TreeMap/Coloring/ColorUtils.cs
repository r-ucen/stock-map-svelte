using System.Drawing;
using System.Numerics;

namespace StockMapSvelte.Application.Services.TreeMap.Coloring;

public static class ColorUtils
{
    public static string ToRgbaString(Color color)
    {
        return $"rgba({color.R}, {color.G}, {color.B}, {(color.A / 255.0):0.##})";
    }
    
    public static Color ThreeWayScale(
        double value,
        double bottom,
        double middle,
        double top,
        Color bottomColor,
        Color middleColor,
        Color topColor)
    {
        var clamped = Math.Clamp(value, bottom, top);
        switch (clamped)
        {
            case < 0:
            {
                var ratio = clamped / bottom;
                var alpha = (int)(ratio * 255);
            
                return Color.FromArgb(alpha, bottomColor.R, bottomColor.G, bottomColor.B);
            }
            case > 0:
            {
                if (clamped < middle)
                {
                    var ratio = middle - (clamped / middle);
                    var alpha = (int)(ratio * 255);
                    
                    return Color.FromArgb(alpha, middleColor.R, middleColor.G, middleColor.B);
                }
                else
                {
                    var ratio = (clamped - middle) / (top - middle);
                    var alpha = (int)(ratio * 255);
                    
                    return Color.FromArgb(alpha, topColor.R, topColor.G, topColor.B);
                }
            }
            default:
                return Color.FromArgb(0, 0, 0, 0);
        }
    }
    
    public static Color NegativeIncreasingAlphaTransparentDecreasingAlphaPositive(
        double value,
        double lo,
        double hi,
        Color negative,
        Color positive)
    {
        var clamped = Math.Clamp(value, lo, hi);
        switch (clamped)
        {
            case < 0:
            {
                var ratio = 1 - (clamped / lo); 
                var alpha = (int)((ratio) * 255);
            
                return Color.FromArgb(alpha, negative.R, negative.G, negative.B);
            }
            case > 0:
            {
                var ratio = 1 - (clamped / hi);
                var alpha = (int)((ratio) * 255);
            
                return Color.FromArgb(alpha, positive.R, positive.G, positive.B);
            }
            default:
                return Color.FromArgb(0, 0, 0, 0);
        }
    }

    public static Color NegativeDecreasingAlphaTransparentIncreasingAlphaPositive(
        double value,
        double lo,
        double hi,
        Color negative,
        Color positive)
    {
        if (lo >= 0 || hi <= 0)
        {
            throw new ArgumentException("lo must be less than 0 and hi must be greater than 0.");
        }
        
        var clamped = Math.Clamp(value, lo, hi);
        switch (clamped)
        {
            case < 0:
            {
                var ratio = clamped / lo; 
                var alpha = (int)(ratio * 255);
            
                return Color.FromArgb(alpha, negative.R, negative.G, negative.B);
            }
            case > 0:
            {
                var ratio = clamped / hi;
                var alpha = (int)(ratio * 255);
            
                return Color.FromArgb(alpha, positive.R, positive.G, positive.B);
            }
            default:
                return Color.FromArgb(0, 0, 0, 0);
        }
    }

    public static Color PositiveIncreasingAlpha<T>(T value, T hi, Color color) where T : INumber<T>
    {
        if (value < T.Zero)
        {
            throw new ArgumentException("value must be non-negative.");
        }
        
        var clamped = T.Clamp(value, T.Zero, hi);
        var ratio = Convert.ToDouble(clamped) / Convert.ToDouble(hi);
        
        if (double.IsNaN(ratio))
        {
            throw new Exception("ratio calculation resulted in NaN.");
        }
        
        var alpha = (int)(ratio * 255);
        return Color.FromArgb(alpha, color.R, color.G, color.B);
    }
    
    public static Color PositiveDecreasingAlpha<T>(T value, T lo, T hi, Color color) where T : INumber<T>
    {
        if (value < T.Zero)
        {
            throw new ArgumentException("value must be non-negative.");
        }
        
        var clamped = T.Clamp(value, lo, hi);
        var ratio = 1 - Convert.ToDouble(clamped) / Convert.ToDouble(hi);
        
        if (double.IsNaN(ratio))
        {
            throw new Exception("ratio calculation resulted in NaN.");
        }
        
        var alpha = (int)(ratio * 255);
        return Color.FromArgb(alpha, color.R, color.G, color.B);
    }
}