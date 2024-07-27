using Announcarr.Extensions;
using Announcarr.Utils.Extensions.String;
using Microsoft.Extensions.Options;
using Quartz;

namespace Announcarr.Configurations.Validations;

public class AnnouncarrConfigurationValidator : IValidateOptions<AnnouncarrConfiguration>
{
    public ValidateOptionsResult Validate(string? name, AnnouncarrConfiguration options)
    {
        ValidateOptionsResult intervalValidationResult = ValidateIntervalConfiguration(options);
        ValidateOptionsResult emptyAnnouncementValidationResult = ValidateEmptyAnnouncementConfiguration(options);

        return intervalValidationResult.Merge(emptyAnnouncementValidationResult);
    }

    private static ValidateOptionsResult ValidateIntervalConfiguration(AnnouncarrConfiguration options)
    {
        return options.Interval.AnnouncerRange switch
        {
            AnnouncerRange.Hourly => ValidateHourlyAnnouncerRange(options),
            AnnouncerRange.Daily => ValidateDailyAnnouncerRange(options),
            AnnouncerRange.Weekly => ValidateWeeklyAnnouncerRange(options),
            AnnouncerRange.Monthly => ValidateMonthlyAnnouncerRange(options),
            AnnouncerRange.Yearly => ValidateYearlyAnnouncerRange(options),
            AnnouncerRange.Cron => ValidateCronAnnouncerRange(options),
            AnnouncerRange.Custom => ValidateCustomAnnouncerRange(options),
            _ => ValidateOptionsResult.Fail($"{nameof(options.Interval.AnnouncerRange)} is not supported"),
        };
    }

    private static ValidateOptionsResult ValidateHourlyAnnouncerRange(AnnouncarrConfiguration options)
    {
        return options.Interval.MinuteOfHour switch
        {
            null => ValidateOptionsResult.Fail($"{nameof(options.Interval.MinuteOfHour)} is required when {nameof(options.Interval.AnnouncerRange)} is set to {AnnouncerRange.Hourly}"),
            < 0 or > 59 => ValidateOptionsResult.Fail($"{nameof(options.Interval.MinuteOfHour)} must be an integer value between 0 and 59 (including)."),
            _ => ValidateOptionsResult.Success,
        };
    }

    private static ValidateOptionsResult ValidateDailyAnnouncerRange(AnnouncarrConfiguration options)
    {
        return options.Interval.HourOfDay switch
        {
            null => ValidateOptionsResult.Fail($"{nameof(options.Interval.HourOfDay)} is required when {nameof(options.Interval.AnnouncerRange)} is set to {AnnouncerRange.Daily}"),
            < 0 or > 59 => ValidateOptionsResult.Fail($"{nameof(options.Interval.HourOfDay)} must be an integer value between 0 and 59 (including)."),
            _ => ValidateOptionsResult.Success,
        };
    }

    private static ValidateOptionsResult ValidateWeeklyAnnouncerRange(AnnouncarrConfiguration options)
    {
        return options.Interval.DayOfWeek switch
        {
            null => ValidateOptionsResult.Fail($"{nameof(options.Interval.DayOfWeek)} is required when {nameof(options.Interval.AnnouncerRange)} is set to {AnnouncerRange.Weekly}"),
            DayOfWeek.Sunday or DayOfWeek.Monday or DayOfWeek.Tuesday or DayOfWeek.Wednesday or DayOfWeek.Thursday or DayOfWeek.Friday or DayOfWeek.Saturday => ValidateOptionsResult.Success,
            _ => ValidateOptionsResult.Fail($"{nameof(options.Interval.DayOfWeek)} value is not supported"),
        };
    }

    private static ValidateOptionsResult ValidateMonthlyAnnouncerRange(AnnouncarrConfiguration options)
    {
        return options.Interval.DayOfMonth switch
        {
            null => ValidateOptionsResult.Fail($"{nameof(options.Interval.DayOfMonth)} is required when {nameof(options.Interval.AnnouncerRange)} is set to {AnnouncerRange.Monthly}"),
            < 1 or > 31 => ValidateOptionsResult.Fail($"{nameof(options.Interval.DayOfMonth)} must be an integer value between 1 and 31 (including)."),
            _ => ValidateOptionsResult.Success,
        };
    }

    private static ValidateOptionsResult ValidateYearlyAnnouncerRange(AnnouncarrConfiguration options)
    {
        return options.Interval.MonthOfYear switch
        {
            null => ValidateOptionsResult.Fail($"{nameof(options.Interval.MonthOfYear)} is required when {nameof(options.Interval.AnnouncerRange)} is set to {AnnouncerRange.Monthly}"),
            < 1 or > 12 => ValidateOptionsResult.Fail($"{nameof(options.Interval.MonthOfYear)} must be an integer value between 1 and 12 (including)."),
            _ => ValidateOptionsResult.Success,
        };
    }

    private static ValidateOptionsResult ValidateCronAnnouncerRange(AnnouncarrConfiguration options)
    {
        if (options.Interval.CustomAnnouncerRange is null)
        {
            return ValidateOptionsResult.Fail($"{nameof(options.Interval.CustomAnnouncerRange)} is required when {nameof(options.Interval.AnnouncerRange)} is set to {AnnouncerRange.Custom}");
        }

        if (options.Interval.CustomAnnouncerRange?.CompareTo(TimeSpan.Zero) > 0)
        {
            return ValidateOptionsResult.Success;
        }

        return ValidateOptionsResult.Fail($"{nameof(options.Interval.CustomAnnouncerRange)} must be a positive time span bigger than {TimeSpan.Zero}");
    }

    private static ValidateOptionsResult ValidateCustomAnnouncerRange(AnnouncarrConfiguration options)
    {
        if (options.Interval.CronAnnouncerRange is null)
        {
            return ValidateOptionsResult.Fail($"{nameof(options.Interval.CronAnnouncerRange)} is required when {nameof(options.Interval.AnnouncerRange)} is set to {AnnouncerRange.Cron}");
        }

        if (CronExpression.IsValidExpression(options.Interval.CronAnnouncerRange))
        {
            return ValidateOptionsResult.Success;
        }

        return ValidateOptionsResult.Fail($"{nameof(options.Interval.CronAnnouncerRange)} is not a valid cron expression");
    }

    private static ValidateOptionsResult ValidateEmptyAnnouncementConfiguration(AnnouncarrConfiguration options)
    {
        if (!options.EmptyContractFallback.ExportOnEmptyContract)
        {
            return ValidateOptionsResult.Success;
        }

        if (options.EmptyContractFallback.CustomMessageOnEmptyContract.IsNullOrWhiteSpace())
        {
            return ValidateOptionsResult.Fail(
                $"{nameof(options.EmptyContractFallback.CustomMessageOnEmptyContract)} cannot be empty or whitespace only when {nameof(options.EmptyContractFallback.ExportOnEmptyContract)} is set to true");
        }

        return ValidateOptionsResult.Success;
    }
}