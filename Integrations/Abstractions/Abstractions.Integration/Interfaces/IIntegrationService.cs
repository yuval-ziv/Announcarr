using Announcarr.Integrations.Abstractions.Responses;

namespace Announcarr.Integrations.Abstractions.Interfaces;

public interface IIntegrationService
{
    bool IsEnabled { get; }
    string GetName();
    bool IsGetCalendarEnabled { get; }
    Task<CalendarResponse> GetCalendarAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default);
    bool IsGetRecentlyAddedEnabled { get; }
    Task<RecentlyAddedResponse> GetRecentlyAddedAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default);
    Task GetNewAnnouncementAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default);
}