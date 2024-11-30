using Announcarr.Extensions;
using Announcarr.Utils.Extensions.String;
using Microsoft.Extensions.Options;

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
        (bool isValid, string? failedFieldName, string? failedReason) = options.Interval.AnnouncarrRange switch
        {
            AnnouncarrRange.Hourly => ValidateHourlyAnnouncarrRange(options),
            AnnouncarrRange.Daily => ValidateDailyAnnouncarrRange(options),
            AnnouncarrRange.Weekly => ValidateWeeklyAnnouncarrRange(options),
            AnnouncarrRange.Monthly => ValidateMonthlyAnnouncarrRange(options),
            AnnouncarrRange.Yearly => ValidateYearlyAnnouncarrRange(options),
            _ => (false, nameof(options.Interval.AnnouncarrRange), "value is not supported"),
        };

        if (isValid)
        {
            return ValidateOptionsResult.Success;
        }

        return ValidateOptionsResult.Fail($"{failedFieldName} {failedReason}. {nameof(options.Interval.AnnouncarrRange)} is set to {options.Interval.AnnouncarrRange}");
    }

    private static (bool IsValid, string? FailedFieldName, string? FailedReason) ValidateHourlyAnnouncarrRange(AnnouncarrConfiguration options)
    {
        return options.Interval.MinuteOfHour switch
        {
            null => Failed(nameof(options.Interval.MinuteOfHour), "is required"),
            < 0 or > 59 => Failed(nameof(options.Interval.MinuteOfHour), "must be an integer value between 0 and 59 (including)"),
            _ => Valid,
        };
    }

    private static (bool IsValid, string? FailedFieldName, string? FailedReason) ValidateDailyAnnouncarrRange(AnnouncarrConfiguration options)
    {
        (bool IsValid, string? FailedFieldName, string? FailedReason) hourlyValidationResult = ValidateHourlyAnnouncarrRange(options);

        if (!hourlyValidationResult.IsValid)
        {
            return hourlyValidationResult;
        }

        return options.Interval.HourOfDay switch
        {
            null => Failed(nameof(options.Interval.HourOfDay), "is required"),
            < 0 or > 23 => Failed(nameof(options.Interval.HourOfDay), "must be an integer value between 0 and 23 (including)"),
            _ => Valid,
        };
    }

    private static (bool IsValid, string? FailedFieldName, string? FailedReason) ValidateWeeklyAnnouncarrRange(AnnouncarrConfiguration options)
    {
        (bool IsValid, string? FailedFieldName, string? FailedReason) dailyValidationResult = ValidateDailyAnnouncarrRange(options);

        if (!dailyValidationResult.IsValid)
        {
            return dailyValidationResult;
        }

        return options.Interval.DayOfWeek switch
        {
            null => Failed(nameof(options.Interval.DayOfWeek), "is required"),
            DayOfWeek.Sunday or DayOfWeek.Monday or DayOfWeek.Tuesday or DayOfWeek.Wednesday or DayOfWeek.Thursday or DayOfWeek.Friday or DayOfWeek.Saturday => Valid,
            _ => Failed(nameof(options.Interval.DayOfWeek), "value is not supported"),
        };
    }

    private static (bool IsValid, string? FailedFieldName, string? FailedReason) ValidateMonthlyAnnouncarrRange(AnnouncarrConfiguration options)
    {
        (bool IsValid, string? FailedFieldName, string? FailedReason) dailyValidationResult = ValidateDailyAnnouncarrRange(options);

        if (!dailyValidationResult.IsValid)
        {
            return dailyValidationResult;
        }

        return options.Interval.DayOfMonth switch
        {
            null => Failed(nameof(options.Interval.DayOfMonth), "is required"),
            < 1 or > 31 => Failed(nameof(options.Interval.DayOfMonth), "must be an integer value between 1 and 31 (including)"),
            _ => Valid,
        };
    }

    private static (bool IsValid, string? FailedFieldName, string? FailedReason) ValidateYearlyAnnouncarrRange(AnnouncarrConfiguration options)
    {
        (bool IsValid, string? FailedFieldName, string? FailedReason) monthlyValidationResult = ValidateMonthlyAnnouncarrRange(options);

        if (!monthlyValidationResult.IsValid)
        {
            return monthlyValidationResult;
        }

        return options.Interval.MonthOfYear switch
        {
            null => Failed(nameof(options.Interval.MonthOfYear), "is required"),
            < 1 or > 12 => Failed(nameof(options.Interval.MonthOfYear), "must be an integer value between 1 and 12 (including)"),
            _ => Valid,
        };
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

    private static (bool IsValid, string? FailedFieldName, string? FailedReason) Valid => (true, null, null);

    private static (bool IsValid, string? FailedFieldName, string? FailedReason) Failed(string failedFieldName, string failedReason) => (false, failedFieldName, failedReason);
}