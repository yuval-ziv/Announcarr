using Announcarr.Configurations;
using NCrontab;

namespace Announcarr.Utils.Extensions;

public static class AnnouncarrIntervalConfigurationExtensions
{
    public static DateTimeOffset GetNextExecution(this AnnouncarrIntervalConfiguration intervalConfiguration) => intervalConfiguration.GetNextExecution(DateTimeOffset.Now);

    public static DateTimeOffset GetNextExecution(this AnnouncarrIntervalConfiguration intervalConfiguration, DateTimeOffset baseTime)
    {
        CrontabSchedule schedule = CrontabSchedule.Parse(intervalConfiguration.ToCron());

        return schedule.GetNextOccurrence(baseTime.DateTime);
    }

    public static (DateTimeOffset Start, DateTimeOffset End) GetLastRange(this AnnouncarrIntervalConfiguration intervalConfiguration)
    {
        DateTimeOffset start = intervalConfiguration.AnnouncarrRange switch
        {
            AnnouncarrRange.Hourly => DateTimeOffset.Now.AddHours(-1),
            AnnouncarrRange.Daily => DateTimeOffset.Now.AddDays(-1),
            AnnouncarrRange.Weekly => DateTimeOffset.Now.AddDays(-7),
            AnnouncarrRange.Monthly => DateTimeOffset.Now.AddMonths(-1),
            AnnouncarrRange.Yearly => DateTimeOffset.Now.AddYears(-1),
            _ => throw new NotImplementedException(),
        };

        return (start, DateTimeOffset.Now);
    }

    public static (DateTimeOffset Start, DateTimeOffset End) GetNextRange(this AnnouncarrIntervalConfiguration intervalConfiguration)
    {
        DateTimeOffset end = intervalConfiguration.GetNextExecution();

        return (DateTimeOffset.Now, end);
    }

    public static string ToCron(this AnnouncarrIntervalConfiguration? intervalConfiguration)
    {
        return intervalConfiguration?.AnnouncarrRange switch
        {
            AnnouncarrRange.Hourly => $"{GetMinuteOfHour(intervalConfiguration)} * * * *",
            AnnouncarrRange.Daily => $"{GetMinuteOfHour(intervalConfiguration)} {GetHourOfDay(intervalConfiguration)} * * *",
            AnnouncarrRange.Weekly => $"{GetMinuteOfHour(intervalConfiguration)} {GetHourOfDay(intervalConfiguration)} * * {GetDayOfWeek(intervalConfiguration)}",
            AnnouncarrRange.Monthly => $"{GetMinuteOfHour(intervalConfiguration)} {GetHourOfDay(intervalConfiguration)} {GetDayOfMonth(intervalConfiguration)} * *",
            AnnouncarrRange.Yearly =>
                $"{GetMinuteOfHour(intervalConfiguration)} {GetHourOfDay(intervalConfiguration)} {GetDayOfMonth(intervalConfiguration)} {GetMonthOfYear(intervalConfiguration)} *",
            _ => throw new ArgumentException($"value of {nameof(intervalConfiguration.AnnouncarrRange)} is unknown", nameof(intervalConfiguration)),
        };
    }

    private static int GetMinuteOfHour(AnnouncarrIntervalConfiguration intervalConfiguration)
    {
        return intervalConfiguration.MinuteOfHour ?? throw new ArgumentException($"{nameof(intervalConfiguration.MinuteOfHour)} must not be null", nameof(intervalConfiguration));
    }

    private static int GetHourOfDay(AnnouncarrIntervalConfiguration intervalConfiguration)
    {
        return intervalConfiguration.HourOfDay ?? throw new ArgumentException($"{nameof(intervalConfiguration.HourOfDay)} must not be null", nameof(intervalConfiguration));
    }

    private static int GetDayOfWeek(AnnouncarrIntervalConfiguration intervalConfiguration)
    {
        return (int?)intervalConfiguration.DayOfWeek ?? throw new ArgumentException($"{nameof(intervalConfiguration.DayOfWeek)} must not be null", nameof(intervalConfiguration));
    }

    private static int? GetDayOfMonth(AnnouncarrIntervalConfiguration intervalConfiguration)
    {
        return intervalConfiguration.DayOfMonth ?? throw new ArgumentException($"{nameof(intervalConfiguration.DayOfMonth)} must not be null", nameof(intervalConfiguration));
    }

    private static int? GetMonthOfYear(AnnouncarrIntervalConfiguration intervalConfiguration)
    {
        return intervalConfiguration.MonthOfYear ?? throw new ArgumentException($"{nameof(intervalConfiguration.MonthOfYear)} must not be null", nameof(intervalConfiguration));
    }
}