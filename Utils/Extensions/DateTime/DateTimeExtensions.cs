namespace Announcarr.Utils.Extensions.DateTime;

public static class DateTimeExtensions
{
    public static bool Between(this DateTimeOffset current, DateTimeOffset from, DateTimeOffset to)
    {
        return current > from && current < to;
    }

    public static bool Between(this System.DateTime current, DateTimeOffset from, DateTimeOffset to) => new DateTimeOffset(current).Between(from, to);
}