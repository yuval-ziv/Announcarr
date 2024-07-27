using Announcarr.Abstractions.Contracts;

namespace Announcarr.Services;

public interface ICalendarService
{
    Task<CalendarContract> GetAllCalendarItemsAsync(DateTimeOffset? start, DateTimeOffset? end, bool? export = false, CancellationToken cancellationToken = default);
    Task<RecentlyAddedContract> GetAllRecentlyAddedItemsAsync(DateTimeOffset? start, DateTimeOffset? end, bool? export = false, CancellationToken cancellationToken = default);
}