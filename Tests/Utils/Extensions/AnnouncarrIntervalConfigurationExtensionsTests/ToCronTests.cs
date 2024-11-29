using Announcarr.Configurations;
using Announcarr.Utils.Extensions;
using FluentAssertions;
using NCrontab;

namespace Announcarr.Test.Utils.Extensions.AnnouncarrIntervalConfigurationExtensionsTests;

public class ToCronTests
{
    [Theory, MemberData(nameof(ValidIntervalsData))]
    public void When_ToCronCalled_Given_DifferentValidIntervals_Then_ReturnValidCron(AnnouncarrIntervalConfiguration intervalConfiguration, string expectedCron)
    {
        string cron = intervalConfiguration.ToCron();

        cron.Should().Be(expectedCron);
        
        CrontabSchedule crontabSchedule = CrontabSchedule.TryParse(cron);

        crontabSchedule.Should().NotBeNull();
    }

    [Theory, MemberData(nameof(InvalidIntervalsData))]
    public void When_ToCronCalled_Given_DifferentInvalidIntervals_Then_ThrowException(AnnouncarrIntervalConfiguration intervalConfiguration, string missingValue)
    {
        intervalConfiguration.Invoking(s => s.ToCron())
            .Should()
            .Throw<ArgumentException>()
            .WithMessage($"{missingValue} must not be null (Parameter 'intervalConfiguration')");
    }

    [Fact]
    public void When_ToCronCalled_Given_NullIntervalConfiguration_Then_ThrowException()
    {
        AnnouncarrIntervalConfiguration? intervalConfiguration = null;
        Action action = () => intervalConfiguration.ToCron();
        action.Should()
            .Throw<ArgumentException>()
            .WithMessage($"value of {nameof(AnnouncarrIntervalConfiguration.AnnouncarrRange)} is unknown (Parameter 'intervalConfiguration')");
    }

    public static TheoryData<AnnouncarrIntervalConfiguration, string> ValidIntervalsData => new()
    {
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Hourly,
                MinuteOfHour = 5,
            },
            "5 * * * *"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Hourly,
                MinuteOfHour = 23,
            },
            "23 * * * *"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Daily,
                MinuteOfHour = 5,
                HourOfDay = 5,
            },
            "5 5 * * *"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Daily,
                MinuteOfHour = 23,
                HourOfDay = 5,
            },
            "23 5 * * *"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Daily,
                MinuteOfHour = 5,
                HourOfDay = 23,
            },
            "5 23 * * *"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Daily,
                MinuteOfHour = 23,
                HourOfDay = 23,
            },
            "23 23 * * *"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Weekly,
                MinuteOfHour = 5,
                HourOfDay = 5,
                DayOfWeek = DayOfWeek.Monday,
            },
            "5 5 * * 1"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Weekly,
                MinuteOfHour = 23,
                HourOfDay = 5,
                DayOfWeek = DayOfWeek.Monday,
            },
            "23 5 * * 1"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Weekly,
                MinuteOfHour = 5,
                HourOfDay = 23,
                DayOfWeek = DayOfWeek.Monday,
            },
            "5 23 * * 1"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Weekly,
                MinuteOfHour = 23,
                HourOfDay = 23,
                DayOfWeek = DayOfWeek.Monday,
            },
            "23 23 * * 1"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Weekly,
                MinuteOfHour = 5,
                HourOfDay = 5,
                DayOfWeek = DayOfWeek.Saturday,
            },
            "5 5 * * 6"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Weekly,
                MinuteOfHour = 23,
                HourOfDay = 5,
                DayOfWeek = DayOfWeek.Saturday,
            },
            "23 5 * * 6"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Weekly,
                MinuteOfHour = 5,
                HourOfDay = 23,
                DayOfWeek = DayOfWeek.Saturday,
            },
            "5 23 * * 6"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Weekly,
                MinuteOfHour = 23,
                HourOfDay = 23,
                DayOfWeek = DayOfWeek.Saturday,
            },
            "23 23 * * 6"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Monthly,
                MinuteOfHour = 5,
                HourOfDay = 5,
                DayOfMonth = 5,
            },
            "5 5 5 * *"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Monthly,
                MinuteOfHour = 23,
                HourOfDay = 5,
                DayOfMonth = 5,
            },
            "23 5 5 * *"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Monthly,
                MinuteOfHour = 5,
                HourOfDay = 23,
                DayOfMonth = 5,
            },
            "5 23 5 * *"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Monthly,
                MinuteOfHour = 23,
                HourOfDay = 23,
                DayOfMonth = 5,
            },
            "23 23 5 * *"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Monthly,
                MinuteOfHour = 5,
                HourOfDay = 5,
                DayOfMonth = 23,
            },
            "5 5 23 * *"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Monthly,
                MinuteOfHour = 23,
                HourOfDay = 5,
                DayOfMonth = 23,
            },
            "23 5 23 * *"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Monthly,
                MinuteOfHour = 5,
                HourOfDay = 23,
                DayOfMonth = 23,
            },
            "5 23 23 * *"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Monthly,
                MinuteOfHour = 23,
                HourOfDay = 23,
                DayOfMonth = 23,
            },
            "23 23 23 * *"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Yearly,
                MinuteOfHour = 5,
                HourOfDay = 5,
                DayOfMonth = 5,
                MonthOfYear = 5,
            },
            "5 5 5 5 *"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Yearly,
                MinuteOfHour = 23,
                HourOfDay = 5,
                DayOfMonth = 5,
                MonthOfYear = 5,
            },
            "23 5 5 5 *"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Yearly,
                MinuteOfHour = 5,
                HourOfDay = 23,
                DayOfMonth = 5,
                MonthOfYear = 5,
            },
            "5 23 5 5 *"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Yearly,
                MinuteOfHour = 23,
                HourOfDay = 23,
                DayOfMonth = 5,
                MonthOfYear = 5,
            },
            "23 23 5 5 *"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Yearly,
                MinuteOfHour = 5,
                HourOfDay = 5,
                DayOfMonth = 23,
                MonthOfYear = 5,
            },
            "5 5 23 5 *"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Yearly,
                MinuteOfHour = 23,
                HourOfDay = 5,
                DayOfMonth = 23,
                MonthOfYear = 5,
            },
            "23 5 23 5 *"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Yearly,
                MinuteOfHour = 5,
                HourOfDay = 23,
                DayOfMonth = 23,
                MonthOfYear = 5,
            },
            "5 23 23 5 *"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Yearly,
                MinuteOfHour = 23,
                HourOfDay = 23,
                DayOfMonth = 23,
                MonthOfYear = 5,
            },
            "23 23 23 5 *"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Yearly,
                MinuteOfHour = 5,
                HourOfDay = 5,
                DayOfMonth = 5,
                MonthOfYear = 11,
            },
            "5 5 5 11 *"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Yearly,
                MinuteOfHour = 23,
                HourOfDay = 5,
                DayOfMonth = 5,
                MonthOfYear = 11,
            },
            "23 5 5 11 *"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Yearly,
                MinuteOfHour = 5,
                HourOfDay = 23,
                DayOfMonth = 5,
                MonthOfYear = 11,
            },
            "5 23 5 11 *"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Yearly,
                MinuteOfHour = 23,
                HourOfDay = 23,
                DayOfMonth = 5,
                MonthOfYear = 11,
            },
            "23 23 5 11 *"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Yearly,
                MinuteOfHour = 5,
                HourOfDay = 5,
                DayOfMonth = 23,
                MonthOfYear = 11,
            },
            "5 5 23 11 *"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Yearly,
                MinuteOfHour = 23,
                HourOfDay = 5,
                DayOfMonth = 23,
                MonthOfYear = 11,
            },
            "23 5 23 11 *"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Yearly,
                MinuteOfHour = 5,
                HourOfDay = 23,
                DayOfMonth = 23,
                MonthOfYear = 11,
            },
            "5 23 23 11 *"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Yearly,
                MinuteOfHour = 23,
                HourOfDay = 23,
                DayOfMonth = 23,
                MonthOfYear = 11,
            },
            "23 23 23 11 *"
        },
    };

    public static TheoryData<AnnouncarrIntervalConfiguration, string> InvalidIntervalsData => new()
    {
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Hourly,
                MinuteOfHour = null,
            },
            "MinuteOfHour"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Daily,
                MinuteOfHour = null,
                HourOfDay = null,
            },
            "MinuteOfHour"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Daily,
                MinuteOfHour = 13,
                HourOfDay = null,
            },
            "HourOfDay"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Weekly,
                MinuteOfHour = null,
                HourOfDay = null,
                DayOfWeek = null,
            },
            "MinuteOfHour"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Weekly,
                MinuteOfHour = 13,
                HourOfDay = null,
                DayOfWeek = null,
            },
            "HourOfDay"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Weekly,
                MinuteOfHour = 13,
                HourOfDay = 13,
                DayOfWeek = null,
            },
            "DayOfWeek"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Monthly,
                MinuteOfHour = null,
                HourOfDay = null,
                DayOfMonth = null,
            },
            "MinuteOfHour"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Monthly,
                MinuteOfHour = 13,
                HourOfDay = null,
                DayOfMonth = null,
            },
            "HourOfDay"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Monthly,
                MinuteOfHour = 13,
                HourOfDay = 13,
                DayOfMonth = null,
            },
            "DayOfMonth"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Yearly,
                MinuteOfHour = null,
                HourOfDay = null,
                DayOfMonth = null,
                MonthOfYear = null,
            },
            "MinuteOfHour"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Yearly,
                MinuteOfHour = 13,
                HourOfDay = null,
                DayOfMonth = null,
                MonthOfYear = null,
            },
            "HourOfDay"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Yearly,
                MinuteOfHour = 13,
                HourOfDay = 13,
                DayOfMonth = null,
                MonthOfYear = null,
            },
            "DayOfMonth"
        },
        {
            new AnnouncarrIntervalConfiguration
            {
                AnnouncarrRange = AnnouncarrRange.Yearly,
                MinuteOfHour = 13,
                HourOfDay = 13,
                DayOfMonth = 13,
                MonthOfYear = null,
            },
            "MonthOfYear"
        },
    };
}