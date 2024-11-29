namespace Announcarr.Abstractions.Contracts;

public class ForecastContract : BaseAnnouncement
{
    public List<BaseItem> Items { get; set; } = [];
    public override bool IsEmpty => Items.Count == 0;
    public override AnnouncementType AnnouncementType => AnnouncementType.Forecast;

    public static ForecastContract Merge(ForecastContract first, ForecastContract second)
    {
        return new ForecastContract
        {
            Items = first.Items.Concat(second.Items).ToList(),
            Tags = first.Tags.Concat(second.Tags).ToHashSet(),
        };
    }
}