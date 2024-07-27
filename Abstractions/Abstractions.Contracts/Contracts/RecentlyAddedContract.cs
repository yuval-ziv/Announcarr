namespace Announcarr.Abstractions.Contracts.Contracts;

public class RecentlyAddedContract
{
    public List<NewlyMonitoredItem> NewlyMonitoredItems { get; set; } = [];
    public List<BaseCalendarItem> NewItems { get; set; } = [];

    public static RecentlyAddedContract Merge(RecentlyAddedContract first, RecentlyAddedContract second)
    {
        return new RecentlyAddedContract
        {
            NewlyMonitoredItems = first.NewlyMonitoredItems.Concat(second.NewlyMonitoredItems).ToList(),
            NewItems = first.NewItems.Concat(second.NewItems).ToList(),
        };
    }
}