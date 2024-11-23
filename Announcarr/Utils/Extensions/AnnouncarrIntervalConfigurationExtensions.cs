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

    public static string ToCron(this AnnouncarrIntervalConfiguration intervalConfiguration)
    {
        return intervalConfiguration.AnnouncarrRange switch
        {
            AnnouncarrRange.Hourly => $"{intervalConfiguration.MinuteOfHour} * * * *",
            AnnouncarrRange.Daily => $"{intervalConfiguration.MinuteOfHour} {intervalConfiguration.HourOfDay} * * *",
            AnnouncarrRange.Weekly => $"{intervalConfiguration.MinuteOfHour} {intervalConfiguration.HourOfDay} * * {(int)intervalConfiguration.DayOfWeek!}",
            AnnouncarrRange.Monthly => $"{intervalConfiguration.MinuteOfHour} {intervalConfiguration.HourOfDay} {intervalConfiguration.DayOfMonth} * *",
            AnnouncarrRange.Yearly => $"{intervalConfiguration.MinuteOfHour} {intervalConfiguration.HourOfDay} {intervalConfiguration.DayOfMonth} {intervalConfiguration.MonthOfYear} *",
            _ => throw new NotImplementedException(),
        };
    }
}