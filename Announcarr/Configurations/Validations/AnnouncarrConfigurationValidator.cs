using Announcarr.Extensions;
using Announcarr.Utils.Extensions.String;
using Microsoft.Extensions.Options;
using NCrontab;

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
        return options.Interval.AnnouncarrRange switch
        {
            AnnouncarrRange.Hourly => ValidateHourlyAnnouncarrRange(options),
            AnnouncarrRange.Daily => ValidateDailyAnnouncarrRange(options),
            AnnouncarrRange.Weekly => ValidateWeeklyAnnouncarrRange(options),
            AnnouncarrRange.Monthly => ValidateMonthlyAnnouncarrRange(options),
            AnnouncarrRange.Yearly => ValidateYearlyAnnouncarrRange(options),
            AnnouncarrRange.Cron => ValidateCronAnnouncarrRange(options),
            _ => ValidateOptionsResult.Fail($"{nameof(options.Interval.AnnouncarrRange)} is not supported"),
        };
    }

    private static ValidateOptionsResult ValidateHourlyAnnouncarrRange(AnnouncarrConfiguration options)
    {
        return options.Interval.MinuteOfHour switch
        {
            null => ValidateOptionsResult.Fail($"{nameof(options.Interval.MinuteOfHour)} is required when {nameof(options.Interval.AnnouncarrRange)} is set to {AnnouncarrRange.Hourly}"),
            < 0 or > 59 => ValidateOptionsResult.Fail($"{nameof(options.Interval.MinuteOfHour)} must be an integer value between 0 and 59 (including)."),
            _ => ValidateOptionsResult.Success,
        };
    }

    private static ValidateOptionsResult ValidateDailyAnnouncarrRange(AnnouncarrConfiguration options)
    {
        ValidateOptionsResult hourlyValidationResult = ValidateHourlyAnnouncarrRange(options);

        if (hourlyValidationResult.Failed)
        {
            return hourlyValidationResult;
        }

        return options.Interval.HourOfDay switch
        {
            null => ValidateOptionsResult.Fail($"{nameof(options.Interval.HourOfDay)} is required when {nameof(options.Interval.AnnouncarrRange)} is set to {AnnouncarrRange.Daily}"),
            < 0 or > 59 => ValidateOptionsResult.Fail($"{nameof(options.Interval.HourOfDay)} must be an integer value between 0 and 59 (including)."),
            _ => ValidateOptionsResult.Success,
        };
    }

    private static ValidateOptionsResult ValidateWeeklyAnnouncarrRange(AnnouncarrConfiguration options)
    {
        ValidateOptionsResult dailyValidationResult = ValidateDailyAnnouncarrRange(options);

        if (dailyValidationResult.Failed)
        {
            return dailyValidationResult;
        }

        return options.Interval.DayOfWeek switch
        {
            null => ValidateOptionsResult.Fail($"{nameof(options.Interval.DayOfWeek)} is required when {nameof(options.Interval.AnnouncarrRange)} is set to {AnnouncarrRange.Weekly}"),
            DayOfWeek.Sunday or DayOfWeek.Monday or DayOfWeek.Tuesday or DayOfWeek.Wednesday or DayOfWeek.Thursday or DayOfWeek.Friday or DayOfWeek.Saturday => ValidateOptionsResult.Success,
            _ => ValidateOptionsResult.Fail($"{nameof(options.Interval.DayOfWeek)} value is not supported"),
        };
    }

    private static ValidateOptionsResult ValidateMonthlyAnnouncarrRange(AnnouncarrConfiguration options)
    {
        ValidateOptionsResult dailyValidationResult = ValidateDailyAnnouncarrRange(options);

        if (dailyValidationResult.Failed)
        {
            return dailyValidationResult;
        }

        return options.Interval.DayOfMonth switch
        {
            null => ValidateOptionsResult.Fail($"{nameof(options.Interval.DayOfMonth)} is required when {nameof(options.Interval.AnnouncarrRange)} is set to {AnnouncarrRange.Monthly}"),
            < 1 or > 31 => ValidateOptionsResult.Fail($"{nameof(options.Interval.DayOfMonth)} must be an integer value between 1 and 31 (including)."),
            _ => ValidateOptionsResult.Success,
        };
    }

    private static ValidateOptionsResult ValidateYearlyAnnouncarrRange(AnnouncarrConfiguration options)
    {
        ValidateOptionsResult dailyValidationResult = ValidateDailyAnnouncarrRange(options);

        if (dailyValidationResult.Failed)
        {
            return dailyValidationResult;
        }

        return options.Interval.MonthOfYear switch
        {
            null => ValidateOptionsResult.Fail($"{nameof(options.Interval.MonthOfYear)} is required when {nameof(options.Interval.AnnouncarrRange)} is set to {AnnouncarrRange.Monthly}"),
            < 1 or > 12 => ValidateOptionsResult.Fail($"{nameof(options.Interval.MonthOfYear)} must be an integer value between 1 and 12 (including)."),
            _ => ValidateOptionsResult.Success,
        };
    }

    private static ValidateOptionsResult ValidateCronAnnouncarrRange(AnnouncarrConfiguration options)
    {
        if (options.Interval.CronAnnouncarrRange is null)
        {
            return ValidateOptionsResult.Fail($"{nameof(options.Interval.CronAnnouncarrRange)} is required when {nameof(options.Interval.AnnouncarrRange)} is set to {AnnouncarrRange.Cron}");
        }

        if (CrontabSchedule.TryParse(options.Interval.CronAnnouncarrRange) is not null)
        {
            return ValidateOptionsResult.Success;
        }

        return ValidateOptionsResult.Fail($"{nameof(options.Interval.CronAnnouncarrRange)} is not a valid cron expression");
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