using StockMapSvelte.Application.Enums;

namespace StockMapSvelte.Application.Services.TreeMap.Formatting;

public class DateDescriptionFormatter
{
    public static string DateCaseDescription(DateTimeOffset date, MapMetric metric)
    {
        var now = DateTimeOffset.UtcNow;
        var differenceInDays = (date.Date - now.Date).Days;
        
        var differenceInMinutes = (date - now).TotalMinutes;
                
        if (differenceInDays < -90 || differenceInDays > 360)
        {
            return "\n-";
        }
                
        var dateString = $"{date:d/M/yyyy dddd}";

        if (differenceInDays == 0)
        {
            if (metric != MapMetric.EarningsDate) return $"\n {dateString}\n" + $"Today";
            
            var timeString = string.Empty;
            
            switch (differenceInMinutes)
            {
                case > 0 and < 60:
                    timeString = $"in {(int)differenceInMinutes} minutes";
                    break;
                case > 0:
                {
                    var hours = (int)(differenceInMinutes / 60);
                    var minutes = (int)(differenceInMinutes % 60);
                    timeString = minutes == 0
                        ? $"in {hours} hours"
                        : $"in {hours} hours and {minutes} minutes";
                    break;
                }
                case < 0:
                {
                    var absDifferenceInMinutes = -differenceInMinutes;
                    if (absDifferenceInMinutes < 60)
                    {
                        timeString = $"{(int)absDifferenceInMinutes} minutes ago";
                    }
                    else
                    {
                        var hours = (int)(absDifferenceInMinutes / 60);
                        var minutes = (int)(absDifferenceInMinutes % 60);
                        timeString = minutes == 0
                            ? $"{hours} hours ago"
                            : $"{hours} hours and {minutes} minutes ago";
                    }

                    break;
                }
            }

            return $"\n {dateString}\n" + $"Today\n{timeString}";
        }
                
        if (differenceInDays > 0)
        {
            return $"\n {dateString}\nin " + $"{(int)differenceInDays} days";
        }
                
        return $"\n {dateString}\n" + $"{-(int)differenceInDays} days ago";
    }
}