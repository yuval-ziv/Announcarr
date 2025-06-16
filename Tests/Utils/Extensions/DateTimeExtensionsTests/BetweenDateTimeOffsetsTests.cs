using Announcarr.Utils.Extensions.DateTime;
using Shouldly;

namespace DateTimeExtensionsTests;

public class BetweenDateTimeOffsetsTests
{
    [Theory]
    [MemberData(nameof(BetweenData))]
    public void When_BetweenCalled_Given_DateTimeOffsetBetweenOtherDateTimeOffsets_Then_ReturnTrue(DateTimeOffset current, DateTimeOffset from, DateTimeOffset to)
    {
        bool result = current.Between(from, to);

        result.ShouldBeTrue();
    }

    [Theory]
    [MemberData(nameof(NotBetweenData))]
    public void When_BetweenCalled_Given_DateTimeOffsetNotBetweenOtherDateTimeOffsets_Then_ReturnFalse(DateTimeOffset current, DateTimeOffset from, DateTimeOffset to)
    {
        bool result = current.Between(from, to);

        result.ShouldBeFalse();
    }

    public static TheoryData<DateTimeOffset, DateTimeOffset, DateTimeOffset> BetweenData() => new()
    {
        {
            new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeZoneInfo.Utc.BaseUtcOffset),
            new DateTimeOffset(1000, 1, 1, 0, 0, 0, TimeZoneInfo.Utc.BaseUtcOffset),
            DateTimeOffset.Now
        },
        {
            new DateTimeOffset(2021, 12, 25, 12, 20, 0, TimeZoneInfo.Utc.BaseUtcOffset),
            new DateTimeOffset(1969, 7, 16, 13, 32, 0, TimeZoneInfo.Utc.BaseUtcOffset),
            DateTimeOffset.Now
        },
        {
            DateTimeOffset.Now,
            DateTimeOffset.MinValue,
            DateTimeOffset.MaxValue
        },
    };

    public static TheoryData<DateTimeOffset, DateTimeOffset, DateTimeOffset> NotBetweenData() => new()
    {
        {
            new DateTimeOffset(1000, 1, 1, 0, 0, 0, TimeZoneInfo.Utc.BaseUtcOffset.Add(TimeSpan.FromHours(1))),
            new DateTimeOffset(1000, 1, 1, 0, 0, 0, TimeZoneInfo.Utc.BaseUtcOffset),
            DateTimeOffset.Now
        },
        {
            new DateTimeOffset(1000, 1, 1, 0, 0, 0, TimeZoneInfo.Utc.BaseUtcOffset),
            new DateTimeOffset(1001, 1, 1, 0, 0, 0, TimeZoneInfo.Utc.BaseUtcOffset),
            DateTimeOffset.Now
        },
        {
            DateTimeOffset.Now,
            DateTimeOffset.MaxValue,
            DateTimeOffset.MinValue
        },
    };
}