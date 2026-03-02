using System.Drawing;

namespace StockMapSvelte.Application.Services.TreeMap.Coloring;

public static class DateColorScale
{
    public static string DateCaseColor(DateTimeOffset date)
    {
        var now = DateTime.UtcNow;
        var differenceInDays = (date.DateTime - now).TotalDays;
                
        if (differenceInDays < -90 || differenceInDays > 360)
        {
            return ColorUtils.ToRgbaString(Color.FromArgb(0, 0, 0, 0));
        }
                
        const int hiDividendDays = 90;
        const int loDividendDays = -90;

        try
        {
            return ColorUtils.ToRgbaString(ColorUtils.NegativeIncreasingAlphaTransparentDecreasingAlphaPositive(
                differenceInDays,
                loDividendDays,
                hiDividendDays,
                Color.FromArgb(255, 255, 0, 0),
                Color.FromArgb(255, 0, 128, 0)
            ));
        }
        catch (Exception)
        {
            return ColorUtils.ToRgbaString(Color.FromArgb(0, 0, 0, 0));
        }
    }
}