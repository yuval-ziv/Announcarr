using Announcarr.Utils.Extensions.DateTime;
using Shouldly;

namespace DateTimeExtensionsTests;

public class BetweenDateTimeTests
{
    [Theory]
    [MemberData(nameof(BetweenData))]
    public void When_BetweenCalled_Given_DateTimeBetweenOtherDateTimes_Then_ReturnTrue(DateTime current, DateTime from, DateTime to)
    {
        bool result = current.Between(from, to);

        result.ShouldBeTrue();
    }

    [Theory]
    [MemberData(nameof(NotBetweenData))]
    public void When_BetweenCalled_Given_DateTimeNotBetweenOtherDateTimes_Then_ReturnFalse(DateTime current, DateTime from, DateTime to)
    {
        bool result = current.Between(from, to);

        result.ShouldBeFalse();
    }

    public static TheoryData<DateTime, DateTime, DateTime> BetweenData() => new()
    {
        {
            new DateTime(2000, 1, 1, 0, 0, 0),
            new DateTime(1000, 1, 1, 0, 0, 0),
            DateTime.Now
        },
        {
            new DateTime(2021, 12, 25, 12, 20, 0),
            new DateTime(1969, 7, 16, 13, 32, 0),
            DateTime.Now
        },
        {
            DateTime.Now,
            DateTime.MinValue.AddDays(1),
            DateTime.MaxValue.AddDays(-1)
        },
    };

    public static TheoryData<DateTime, DateTime, DateTime> NotBetweenData() => new()
    {
        {
            DateTime.Now.AddDays(1),
            new DateTime(1000, 1, 1, 0, 0, 0),
            DateTime.Now
        },
        {
            new DateTime(1000, 1, 1, 0, 0, 0),
            new DateTime(1001, 1, 1, 0, 0, 0),
            DateTime.Now
        },
        {
            DateTime.Now,
            DateTime.MaxValue.AddDays(-1),
            DateTime.MinValue.AddDays(1)
        },
    };
}