namespace Announcarr.Abstractions.Contracts.Contracts;

public class RecentlyAddedResponse
{
    public List<NewlyMonitoredItem> NewlyMonitoredItems { get; set; } = [];
    public List<BaseCalendarItem> NewItems { get; set; } = [];

    public static RecentlyAddedResponse Merge(RecentlyAddedResponse first, RecentlyAddedResponse second)
    {
        return new RecentlyAddedResponse
        {
            NewlyMonitoredItems = first.NewlyMonitoredItems.Concat(second.NewlyMonitoredItems).ToList(),
            NewItems = first.NewItems.Concat(second.NewItems).ToList(),
        };
    }
}