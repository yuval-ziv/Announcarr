namespace Announcarr.Abstractions.Contracts;

public class RecentlyAddedContract : BaseAnnouncement
{
    public List<NewlyMonitoredItem> NewlyMonitoredItems { get; set; } = [];
    public List<BaseCalendarItem> NewItems { get; set; } = [];
    public override bool IsEmpty => NewlyMonitoredItems.Count == 0 && NewItems.Count == 0;
    public override AnnouncementType AnnouncementType => AnnouncementType.RecentlyAdded;

    public static RecentlyAddedContract Merge(RecentlyAddedContract first, RecentlyAddedContract second)
    {
        return new RecentlyAddedContract
        {
            NewlyMonitoredItems = first.NewlyMonitoredItems.Concat(second.NewlyMonitoredItems).ToList(),
            NewItems = first.NewItems.Concat(second.NewItems).ToList(),
            Tags = first.Tags.Concat(second.Tags).ToHashSet(),
        };
    }
}