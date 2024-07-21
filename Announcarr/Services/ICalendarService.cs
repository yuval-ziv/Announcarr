using Announcarr.Integrations.Abstractions.Responses;

namespace Announcarr.Services;

public interface ICalendarService
{
    Task<CalendarResponse> GetAllCalendarItemsAsync(DateTimeOffset? start, DateTimeOffset? end, CancellationToken cancellationToken = default);
    Task<RecentlyAddedResponse> GetAllRecentlyAddedItemsAsync(DateTimeOffset? start, DateTimeOffset? end, CancellationToken cancellationToken = default);
}