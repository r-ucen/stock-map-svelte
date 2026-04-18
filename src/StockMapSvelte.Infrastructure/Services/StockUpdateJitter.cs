namespace StockMapSvelte.Infrastructure.Services;

public class StockUpdateJitter
{
    public static ValueTuple<bool, int> GetVariableDelayInMinutes()
    {
        var delayMinutes = Random.Shared.Next(25, 45);
        var executeWork = !(IsWeekend(DateTimeOffset.UtcNow) || IsDeepNight(DateTimeOffset.UtcNow));
        
        return (executeWork, delayMinutes);
    }

    private static bool IsWeekend(DateTimeOffset date)
    {
        return date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
    }

    private static bool IsDeepNight(DateTimeOffset date)
    {
        var hour = date.Hour;
        return hour is < 6 or >= 24;
    }
}