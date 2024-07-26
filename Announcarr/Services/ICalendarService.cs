using Announcarr.Abstractions.Contracts.Contracts;

namespace Announcarr.Services;

public interface ICalendarService
{
    Task<CalendarResponse> GetAllCalendarItemsAsync(DateTimeOffset? start, DateTimeOffset? end, bool? export = false, CancellationToken cancellationToken = default);
    Task<RecentlyAddedResponse> GetAllRecentlyAddedItemsAsync(DateTimeOffset? start, DateTimeOffset? end, bool? export = false, CancellationToken cancellationToken = default);
}