using Cronos;
using NodaTime;

namespace Application.UnitTests;

public class CronDeepDiveTests
{
    [Fact]
    public void TestCronLibraryDirectly()
    {
        // Test the CRON library directly to understand its behavior
        var cronExpression = CronExpression.Parse("0 * * * 0"); // Every hour on Sunday
        
        // Sunday, Jan 14, 2024
        var sundayMidnight = new DateTime(2024, 1, 14, 0, 0, 0, DateTimeKind.Utc);
        var sundayNoon = new DateTime(2024, 1, 14, 12, 0, 0, DateTimeKind.Utc);
        
        // Saturday, Jan 13, 2024
        var saturdayMidnight = new DateTime(2024, 1, 13, 0, 0, 0, DateTimeKind.Utc);
        var saturdayNoon = new DateTime(2024, 1, 13, 12, 0, 0, DateTimeKind.Utc);
        
        // Test getting next occurrence from different starting points
        var fromSatMidnight = cronExpression.GetNextOccurrence(saturdayMidnight, TimeZoneInfo.Utc);
        var fromSatNoon = cronExpression.GetNextOccurrence(saturdayNoon, TimeZoneInfo.Utc);
        var fromSunMidnight = cronExpression.GetNextOccurrence(sundayMidnight, TimeZoneInfo.Utc);
        var fromSunNoon = cronExpression.GetNextOccurrence(sundayNoon, TimeZoneInfo.Utc);
        
        // Test GetOccurrences to see all occurrences in a range
        var sundayOccurrences = cronExpression.GetOccurrences(sundayMidnight, sundayMidnight.AddDays(1), TimeZoneInfo.Utc);
        
        // Output for debugging
        Console.WriteLine($"Saturday midnight ({saturdayMidnight:yyyy-MM-dd HH:mm:ss dddd}): Next occurrence = {fromSatMidnight?.ToString("yyyy-MM-dd HH:mm:ss dddd") ?? "null"}");
        Console.WriteLine($"Saturday noon ({saturdayNoon:yyyy-MM-dd HH:mm:ss dddd}): Next occurrence = {fromSatNoon?.ToString("yyyy-MM-dd HH:mm:ss dddd") ?? "null"}");
        Console.WriteLine($"Sunday midnight ({sundayMidnight:yyyy-MM-dd HH:mm:ss dddd}): Next occurrence = {fromSunMidnight?.ToString("yyyy-MM-dd HH:mm:ss dddd") ?? "null"}");
        Console.WriteLine($"Sunday noon ({sundayNoon:yyyy-MM-dd HH:mm:ss dddd}): Next occurrence = {fromSunNoon?.ToString("yyyy-MM-dd HH:mm:ss dddd") ?? "null"}");
        
        Console.WriteLine($"\nSunday occurrences count: {sundayOccurrences.Count()}");
        foreach (var occurrence in sundayOccurrences.Take(5))
        {
            Console.WriteLine($"  - {occurrence:yyyy-MM-dd HH:mm:ss dddd}");
        }
        
        // Key assertions
        Assert.True(fromSatMidnight.HasValue, "Should find next occurrence from Saturday midnight");
        Assert.True(fromSatNoon.HasValue, "Should find next occurrence from Saturday noon");
        
        if (fromSatMidnight.HasValue)
        {
            var nextFromSat = fromSatMidnight.Value;
            Assert.Equal(DayOfWeek.Sunday, nextFromSat.DayOfWeek);
            Console.WriteLine($"✓ Next occurrence from Saturday is on Sunday: {nextFromSat:yyyy-MM-dd dddd}");
        }
        
        Assert.True(sundayOccurrences.Any(), "Sunday should have occurrences");
        
        // This is the key test: from Sunday midnight, the next occurrence should be either:
        // 1. Later on Sunday (if looking at 0:00 and next is 1:00)
        // 2. Next Sunday (if looking at 23:59 and next is next week)
        if (fromSunMidnight.HasValue)
        {
            var nextFromSunMidnight = fromSunMidnight.Value;
            Console.WriteLine($"From Sunday midnight, next occurrence: {nextFromSunMidnight:yyyy-MM-dd HH:mm:ss dddd}");
            
            // Since it's "every hour on Sunday", from Sunday 00:00, next should be Sunday 01:00
            if (nextFromSunMidnight.Date == sundayMidnight.Date)
            {
                Console.WriteLine("✓ Next occurrence is later on the same Sunday");
            }
            else
            {
                Console.WriteLine("✗ Next occurrence is on a different day (next Sunday)");
            }
        }
    }
}
