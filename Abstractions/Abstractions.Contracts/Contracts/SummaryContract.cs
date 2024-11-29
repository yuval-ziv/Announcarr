namespace Announcarr.Abstractions.Contracts;

public class SummaryContract : BaseAnnouncement
{
    public List<NewlyMonitoredItem> NewlyMonitoredItems { get; set; } = [];
    public List<BaseItem> NewItems { get; set; } = [];
    public override bool IsEmpty => NewlyMonitoredItems.Count == 0 && NewItems.Count == 0;
    public override AnnouncementType AnnouncementType => AnnouncementType.Summary;

    public static SummaryContract Merge(SummaryContract first, SummaryContract second)
    {
        return new SummaryContract
        {
            NewlyMonitoredItems = first.NewlyMonitoredItems.Concat(second.NewlyMonitoredItems).ToList(),
            NewItems = first.NewItems.Concat(second.NewItems).ToList(),
            Tags = first.Tags.Concat(second.Tags).ToHashSet(),
        };
    }
}