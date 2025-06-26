using Cronos;
using NodaTime;
using Application.Domain.Entities;

namespace Application.Domain.Helpers;

/// <summary>
/// Utility for evaluating CRON schedules and determining if a special is currently active
/// </summary>
public static class CronScheduleEvaluator
{
    /// <summary>
    /// Determines if a special is currently active based on its schedule and time constraints
    /// </summary>
    /// <param name="special">The special entity to evaluate</param>
    /// <param name="currentDateTime">The current date and time to evaluate against</param>
    /// <returns>True if the special is currently active, false otherwise</returns>
    public static bool IsSpecialCurrentlyActive(SpecialEntity special, LocalDateTime currentDateTime)
    {
        // First check basic active flag
        if (!special.IsActive)
            return false;

        var currentDate = currentDateTime.Date;
        var currentTime = currentDateTime.TimeOfDay;

        // Check if we're within the date range
        if (currentDate < special.StartDate)
            return false;

        if (special.EndDate.HasValue && currentDate > special.EndDate.Value)
            return false;

        // For non-recurring specials, check if we're on the exact date and within time range
        if (!special.IsRecurring)
        {
            if (currentDate != special.StartDate)
                return false;

            return IsWithinTimeRange(currentTime, special.StartTime, special.EndTime);
        }

        // For recurring specials, evaluate the CRON schedule
        if (!string.IsNullOrWhiteSpace(special.CronSchedule))
        {
            try
            {
                var cronExpression = CronExpression.Parse(special.CronSchedule);
                
                // Convert NodaTime LocalDateTime to System.DateTime for Cronos
                // Cronos requires System.DateTime, so we need this conversion
                var systemDateTime = currentDateTime.ToDateTimeUnspecified();
                
                // Check if today matches the CRON schedule
                var todayMidnight = currentDate.AtMidnight().ToDateTimeUnspecified();
                var nextOccurrence = cronExpression.GetNextOccurrence(todayMidnight, TimeZoneInfo.Utc);

                bool isScheduleActive = false;
                
                if (nextOccurrence.HasValue)
                {
                    var nextDateTime = LocalDateTime.FromDateTime(nextOccurrence.Value);
                    
                    // Check if the next occurrence is today
                    if (nextDateTime.Date == currentDate)
                    {
                        isScheduleActive = true;
                    }
                }

                // If today doesn't match, check if yesterday had an occurrence that might still be running
                if (!isScheduleActive)
                {
                    var yesterdayMidnight = currentDate.PlusDays(-1).AtMidnight().ToDateTimeUnspecified();
                    var yesterdayOccurrence = cronExpression.GetNextOccurrence(yesterdayMidnight, TimeZoneInfo.Utc);
                    
                    if (yesterdayOccurrence.HasValue)
                    {
                        var yesterdayDateTime = LocalDateTime.FromDateTime(yesterdayOccurrence.Value);
                        if (yesterdayDateTime.Date == currentDate.PlusDays(-1))
                        {
                            // Check if the special from yesterday extends into today
                            if (special.EndTime.HasValue && special.StartTime > special.EndTime.Value)
                            {
                                // This is a special that crosses midnight (e.g., 23:00 to 02:00)
                                isScheduleActive = currentTime <= special.EndTime.Value;
                            }
                        }
                    }
                }

                if (!isScheduleActive)
                    return false;

                // If CRON schedule is active, check time range
                return IsWithinTimeRange(currentTime, special.StartTime, special.EndTime);
            }
            catch
            {
                // If CRON parsing fails, fall back to basic time checking
                return IsWithinTimeRange(currentTime, special.StartTime, special.EndTime);
            }
        }

        // If no CRON schedule, just check time range
        return IsWithinTimeRange(currentTime, special.StartTime, special.EndTime);
    }

    /// <summary>
    /// Checks if the current time is within the specified time range
    /// </summary>
    private static bool IsWithinTimeRange(LocalTime currentTime, LocalTime startTime, LocalTime? endTime)
    {
        if (currentTime < startTime)
            return false;

        if (endTime.HasValue && currentTime > endTime.Value)
            return false;

        return true;
    }

    /// <summary>
    /// Gets the next occurrence of a recurring special
    /// </summary>
    public static LocalDateTime? GetNextOccurrence(SpecialEntity special, LocalDateTime fromDateTime)
    {
        if (!special.IsRecurring || string.IsNullOrWhiteSpace(special.CronSchedule))
            return null;

        try
        {
            var cronExpression = CronExpression.Parse(special.CronSchedule);
            var systemDateTime = fromDateTime.ToDateTimeUnspecified();
            
            var nextOccurrence = cronExpression.GetNextOccurrence(systemDateTime, TimeZoneInfo.Utc);
            if (!nextOccurrence.HasValue)
                return null;

            // Convert back to LocalDateTime and combine with special's start time
            var localNextDate = LocalDate.FromDateTime(nextOccurrence.Value);
            return localNextDate + special.StartTime;
        }
        catch
        {
            return null;
        }
    }
}
