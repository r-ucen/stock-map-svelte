namespace StockMapSvelte.Infrastructure.Services;

public static class StockUpdateJitter
{
    public static ValueTuple<bool, int> GetVariableDelayInMinutes(DateTimeOffset date, ref bool isFirstRun)
    {
        var delayMinutes = Random.Shared.Next(25, 45);
        
        if (isFirstRun) { 
            isFirstRun = false;
            return ValueTuple.Create(true, delayMinutes);
        }

        var executeWork = (date.DayOfWeek, date.Hour) switch
        {
            (DayOfWeek.Saturday, < 2) => true,
            (DayOfWeek.Saturday or DayOfWeek.Sunday, _) => false,
            
            (DayOfWeek.Monday, < 2) => false,
            (DayOfWeek.Monday, _) => true,
            
            (_, < 6) => false,
            _ => true
        };
        
        return (executeWork, delayMinutes);
    }
}