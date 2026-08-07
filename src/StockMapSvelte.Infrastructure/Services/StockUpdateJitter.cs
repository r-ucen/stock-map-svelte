namespace StockMapSvelte.Infrastructure.Services;

public class StockUpdateJitter
{
    public static ValueTuple<bool, int> GetVariableDelayInMinutes(ref bool isFirstRun, ref bool isFirstWeekendRun)
    {
        var delayMinutes = Random.Shared.Next(25, 45);
        var executeWork = !(IsWeekend(DateTimeOffset.UtcNow) || IsDeepNight(DateTimeOffset.UtcNow));

        if (isFirstRun) { isFirstRun = false; executeWork = true; }

        if (IsWeekend(DateTimeOffset.UtcNow) && isFirstWeekendRun)
        {
            isFirstWeekendRun = false; executeWork = true;
        }
        else if (!IsWeekend(DateTimeOffset.UtcNow) && !isFirstWeekendRun)
        {
            isFirstWeekendRun = true;
        }

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