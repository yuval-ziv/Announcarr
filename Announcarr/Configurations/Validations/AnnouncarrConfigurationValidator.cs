using Microsoft.Extensions.Options;
using Quartz;

namespace Announcarr.Configurations.Validations;

public class AnnouncarrConfigurationValidator : IValidateOptions<AnnouncarrConfiguration>
{
    public ValidateOptionsResult Validate(string? name, AnnouncarrConfiguration options)
    {
        return options.AnnouncerRange switch
        {
            AnnouncerRange.Hourly => ValidateHourlyAnnouncerRange(options),
            AnnouncerRange.Daily => ValidateDailyAnnouncerRange(options),
            AnnouncerRange.Weekly => ValidateWeeklyAnnouncerRange(options),
            AnnouncerRange.Monthly => ValidateMonthlyAnnouncerRange(options),
            AnnouncerRange.Yearly => ValidateYearlyAnnouncerRange(options),
            AnnouncerRange.Cron => ValidateCronAnnouncerRange(options),
            AnnouncerRange.Custom => ValidateCustomAnnouncerRange(options),
            _ => ValidateOptionsResult.Fail($"{nameof(options.AnnouncerRange)} is not supported"),
        };
    }

    private static ValidateOptionsResult ValidateHourlyAnnouncerRange(AnnouncarrConfiguration options)
    {
        return options.MinuteOfHour switch
        {
            null => ValidateOptionsResult.Fail($"{nameof(options.MinuteOfHour)} is required when {nameof(options.AnnouncerRange)} is set to {AnnouncerRange.Hourly}"),
            < 0 or > 59 => ValidateOptionsResult.Fail($"{nameof(options.MinuteOfHour)} must be an integer value between 0 and 59 (including)."),
            _ => ValidateOptionsResult.Success,
        };
    }

    private static ValidateOptionsResult ValidateDailyAnnouncerRange(AnnouncarrConfiguration options)
    {
        return options.HourOfDay switch
        {
            null => ValidateOptionsResult.Fail($"{nameof(options.HourOfDay)} is required when {nameof(options.AnnouncerRange)} is set to {AnnouncerRange.Daily}"),
            < 0 or > 59 => ValidateOptionsResult.Fail($"{nameof(options.HourOfDay)} must be an integer value between 0 and 59 (including)."),
            _ => ValidateOptionsResult.Success,
        };
    }

    private static ValidateOptionsResult ValidateWeeklyAnnouncerRange(AnnouncarrConfiguration options)
    {
        return options.DayOfWeek switch
        {
            null => ValidateOptionsResult.Fail($"{nameof(options.DayOfWeek)} is required when {nameof(options.AnnouncerRange)} is set to {AnnouncerRange.Weekly}"),
            DayOfWeek.Sunday or DayOfWeek.Monday or DayOfWeek.Tuesday or DayOfWeek.Wednesday or DayOfWeek.Thursday or DayOfWeek.Friday or DayOfWeek.Saturday => ValidateOptionsResult.Success,
            _ => ValidateOptionsResult.Fail($"{nameof(options.DayOfWeek)} value is not supported"),
        };
    }

    private static ValidateOptionsResult ValidateMonthlyAnnouncerRange(AnnouncarrConfiguration options)
    {
        return options.DayOfMonth switch
        {
            null => ValidateOptionsResult.Fail($"{nameof(options.DayOfMonth)} is required when {nameof(options.AnnouncerRange)} is set to {AnnouncerRange.Monthly}"),
            < 1 or > 31 => ValidateOptionsResult.Fail($"{nameof(options.DayOfMonth)} must be an integer value between 1 and 31 (including)."),
            _ => ValidateOptionsResult.Success,
        };
    }

    private static ValidateOptionsResult ValidateYearlyAnnouncerRange(AnnouncarrConfiguration options)
    {
        return options.MonthOfYear switch
        {
            null => ValidateOptionsResult.Fail($"{nameof(options.MonthOfYear)} is required when {nameof(options.AnnouncerRange)} is set to {AnnouncerRange.Monthly}"),
            < 1 or > 12 => ValidateOptionsResult.Fail($"{nameof(options.MonthOfYear)} must be an integer value between 1 and 12 (including)."),
            _ => ValidateOptionsResult.Success,
        };
    }

    private static ValidateOptionsResult ValidateCronAnnouncerRange(AnnouncarrConfiguration options)
    {
        if (options.CustomAnnouncerRange is null)
        {
            return ValidateOptionsResult.Fail($"{nameof(options.CustomAnnouncerRange)} is required when {nameof(options.AnnouncerRange)} is set to {AnnouncerRange.Custom}");
        }

        if (options.CustomAnnouncerRange?.CompareTo(TimeSpan.Zero) > 0)
        {
            return ValidateOptionsResult.Success;
        }

        return ValidateOptionsResult.Fail($"{nameof(options.CustomAnnouncerRange)} must be a positive time span bigger than {TimeSpan.Zero}");
    }

    private ValidateOptionsResult ValidateCustomAnnouncerRange(AnnouncarrConfiguration options)
    {
        if (options.CronAnnouncerRange is null)
        {
            return ValidateOptionsResult.Fail($"{nameof(options.CronAnnouncerRange)} is required when {nameof(options.AnnouncerRange)} is set to {AnnouncerRange.Cron}");
        }

        if (CronExpression.IsValidExpression(options.CronAnnouncerRange))
        {
            return ValidateOptionsResult.Success;
        }

        return ValidateOptionsResult.Fail($"{nameof(options.CronAnnouncerRange)} is not a valid cron expression");
    }
}